import { ref } from 'vue'
import { api } from '../services/api'

const currentEmployee = ref(null)
const employees = ref([])

export function useEmployeeStore() {
  const loadEmployees = async () => {
    try {
      const response = await api.get('/Employees')
      employees.value = response.data
      if (!currentEmployee.value && employees.value.length > 0) {
        const savedId = localStorage.getItem('currentEmployeeId')
        if (savedId) {
          const saved = employees.value.find(e => e.id === savedId)
          if (saved) {
            currentEmployee.value = saved
            return
          }
        }
        currentEmployee.value = employees.value[0]
      }
    } catch (error) {
      console.error('Ошибка загрузки сотрудников:', error)
    }
  }

  const refreshEmployees = async () => {
    try {
      const response = await api.get('/Employees')
      employees.value = response.data
      
      if (currentEmployee.value) {
        const stillExists = employees.value.find(e => e.id === currentEmployee.value.id)
        if (!stillExists && employees.value.length > 0) {
          currentEmployee.value = employees.value[0]
        }
      } else if (employees.value.length > 0) {
        currentEmployee.value = employees.value[0]
      }
    } catch (error) {
      console.error('Ошибка обновления сотрудников:', error)
    }
  }

  const setEmployee = (employee) => {
    currentEmployee.value = employee
    if (employee) {
      localStorage.setItem('currentEmployeeId', employee.id)
    } else {
      localStorage.removeItem('currentEmployeeId')
    }
  }

  return {
    currentEmployee,
    employees,
    loadEmployees,
    refreshEmployees,  
    setEmployee,
  }
}