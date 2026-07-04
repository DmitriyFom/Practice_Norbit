using System.Data;
using Microsoft.Data.SqlClient;
using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Repositories;

/// <summary>
/// ADO.NET.
/// </summary>
public class AdoNetCarRepository : ICarRepository
{
    private readonly string _connectionString;

    public AdoNetCarRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<List<Car>> GetAllAsync()
    {
        const string sql = @"
            SELECT c.CarID, c.VIN, c.ModelID, c.Color, c.YearOfManufacture, 
                   c.Mileage, c.Price, c.IsSold, c.ArrivalDate, c.DiscountPercent,
                   m.ModelName, b.BrandName
            FROM Cars c
            INNER JOIN Models m ON c.ModelID = m.ModelID
            INNER JOIN Brands b ON m.BrandID = b.BrandID
            ORDER BY c.ArrivalDate DESC";

        var cars = new List<Car>();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            cars.Add(MapCarWithModel(reader));
        }
        return cars;
    }


    public async Task<Guid> GetOrCreateBrandAsync(string brandName)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var findCmd = new SqlCommand(
            "SELECT BrandID FROM Brands WHERE BrandName = @Name AND IsActive = 1",
            connection);
        findCmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = brandName;

        var existingId = await findCmd.ExecuteScalarAsync();
        if (existingId != null)
        {
            return (Guid)existingId;
        }

        using var insertCmd = new SqlCommand(
            @"INSERT INTO Brands (BrandID, BrandName, Country, IsActive, CreatedAt)
          VALUES (@Id, @Name, 'Не указана', 1, GETDATE());
          SELECT @Id",
            connection);

        var newId = Guid.NewGuid();
        insertCmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = newId;
        insertCmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = brandName;

        await insertCmd.ExecuteNonQueryAsync();
        return newId;
    }

    public async Task<Guid> GetOrCreateModelAsync(string modelName, Guid brandId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var findCmd = new SqlCommand(
            "SELECT ModelID FROM Models WHERE ModelName = @Name AND BrandID = @BrandId AND IsActive = 1",
            connection);
        findCmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = modelName;
        findCmd.Parameters.Add("@BrandId", SqlDbType.UniqueIdentifier).Value = brandId;

        var existingId = await findCmd.ExecuteScalarAsync();
        if (existingId != null)
        {
            return (Guid)existingId;
        }

        using var insertCmd = new SqlCommand(
            @"INSERT INTO Models (ModelID, ModelName, BrandID, BodyType, BasePrice, IsActive)
          VALUES (@Id, @Name, @BrandId, 'Не указан', 0, 1);
          SELECT @Id",
            connection);

        var newId = Guid.NewGuid();
        insertCmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = newId;
        insertCmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = modelName;
        insertCmd.Parameters.Add("@BrandId", SqlDbType.UniqueIdentifier).Value = brandId;

        await insertCmd.ExecuteNonQueryAsync();
        return newId;
    }
    public async Task<Car?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT c.CarID, c.VIN, c.ModelID, c.Color, c.YearOfManufacture, 
                   c.Mileage, c.Price, c.IsSold, c.ArrivalDate, c.DiscountPercent,
                   m.ModelName, b.BrandName
            FROM Cars c
            INNER JOIN Models m ON c.ModelID = m.ModelID
            INNER JOIN Brands b ON m.BrandID = b.BrandID
            WHERE c.CarID = @Id";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapCarWithModel(reader);
        }
        return null;
    }


    public async Task<Car> CreateAsync(Car car)
    {
        const string sql = @"
        INSERT INTO Cars (CarID, VIN, ModelID, Color, YearOfManufacture, Mileage, Price, DiscountPercent, ArrivalDate)
        VALUES (@Id, @VIN, @ModelId, @Color, @Year, @Mileage, @Price, @Discount, @ArrivalDate);
        SELECT @Id, @ArrivalDate";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            using var command = new SqlCommand(sql, connection, transaction);
            car.CarId = Guid.NewGuid();
            car.ArrivalDate = DateTime.Now;

            command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = car.CarId;
            command.Parameters.Add("@VIN", SqlDbType.NVarChar, 17).Value = car.VIN;
            command.Parameters.Add("@ModelId", SqlDbType.UniqueIdentifier).Value = car.ModelId;
            command.Parameters.Add("@Color", SqlDbType.NVarChar, 30).Value = car.Color;
            command.Parameters.Add("@Year", SqlDbType.Int).Value = car.YearOfManufacture;
            command.Parameters.Add("@Mileage", SqlDbType.Int).Value = car.Mileage;
            command.Parameters.Add("@Price", SqlDbType.Decimal).Value = car.Price;
            command.Parameters.Add("@Discount", SqlDbType.Decimal).Value =
                (object?)car.DiscountPercent ?? DBNull.Value;
            command.Parameters.Add("@ArrivalDate", SqlDbType.DateTime2).Value = car.ArrivalDate;

            await command.ExecuteNonQueryAsync();
            transaction.Commit();

            return car;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    public async Task<bool> HasRelatedSalesAsync(Guid carId)
    {
        const string sql = "SELECT COUNT(*) FROM Sales WHERE CarID = @CarId";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@CarId", SqlDbType.UniqueIdentifier).Value = carId;

        var count = (int)(await command.ExecuteScalarAsync())!;
        return count > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            const string sql1 = @"
            DELETE FROM SalesServices 
            WHERE SaleID IN (SELECT SaleID FROM Sales WHERE CarID = @CarId)";

            using (var cmd1 = new SqlCommand(sql1, connection, transaction))
            {
                cmd1.Parameters.Add("@CarId", SqlDbType.UniqueIdentifier).Value = id;
                await cmd1.ExecuteNonQueryAsync();
            }

            const string sql2 = "DELETE FROM Sales WHERE CarID = @CarId";

            using (var cmd2 = new SqlCommand(sql2, connection, transaction))
            {
                cmd2.Parameters.Add("@CarId", SqlDbType.UniqueIdentifier).Value = id;
                await cmd2.ExecuteNonQueryAsync();
            }

            const string sql3 = "DELETE FROM Cars WHERE CarID = @CarId";

            using (var cmd3 = new SqlCommand(sql3, connection, transaction))
            {
                cmd3.Parameters.Add("@CarId", SqlDbType.UniqueIdentifier).Value = id;
                int affected = await cmd3.ExecuteNonQueryAsync();

                if (affected == 0)
                {
                    transaction.Rollback();
                    return false;
                }
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    public async Task<bool> UpdateAsync(Car car)
    {
        const string sql = @"
            UPDATE Cars
            SET Color = @Color,
                YearOfManufacture = @Year,
                Mileage = @Mileage,
                Price = @Price,
                DiscountPercent = @Discount
            WHERE CarID = @Id";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = car.CarId;
        command.Parameters.Add("@Color", SqlDbType.NVarChar, 30).Value = car.Color;
        command.Parameters.Add("@Year", SqlDbType.Int).Value = car.YearOfManufacture;
        command.Parameters.Add("@Mileage", SqlDbType.Int).Value = car.Mileage;
        command.Parameters.Add("@Price", SqlDbType.Decimal).Value = car.Price;
        command.Parameters.Add("@Discount", SqlDbType.Decimal).Value =
            (object?)car.DiscountPercent ?? DBNull.Value;

        return await command.ExecuteNonQueryAsync() > 0;
    }


    private static Car MapCarWithModel(SqlDataReader reader)
    {
        return new Car
        {
            CarId = reader.GetGuid(reader.GetOrdinal("CarID")),
            VIN = reader.GetString(reader.GetOrdinal("VIN")),
            ModelId = reader.GetGuid(reader.GetOrdinal("ModelID")),
            Color = reader.GetString(reader.GetOrdinal("Color")),
            YearOfManufacture = reader.GetInt32(reader.GetOrdinal("YearOfManufacture")),
            Mileage = reader.GetInt32(reader.GetOrdinal("Mileage")),
            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
            IsSold = reader.GetBoolean(reader.GetOrdinal("IsSold")),
            ArrivalDate = reader.GetDateTime(reader.GetOrdinal("ArrivalDate")),
            DiscountPercent = reader.IsDBNull(reader.GetOrdinal("DiscountPercent"))
                ? null
                : reader.GetDecimal(reader.GetOrdinal("DiscountPercent")),
            Model = new Model
            {
                ModelName = reader.GetString(reader.GetOrdinal("ModelName")),
                Brand = new Brand
                {
                    BrandName = reader.GetString(reader.GetOrdinal("BrandName"))
                }
            }
        };
    }
}