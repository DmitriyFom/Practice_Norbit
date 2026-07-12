<template>
  <div class="time-entries-view">
    <div class="page-header">
      <div class="page-title">
        <h1>Проводки времени</h1>
        <p class="page-subtitle" v-if="currentEmployee">
          Сотрудник: <strong>{{ currentEmployee.firstName }} {{ currentEmployee.lastName }}</strong>
        </p>
        <p class="page-subtitle" v-else>Выберите сотрудника в боковой панели</p>
      </div>
      <button class="btn btn-primary" @click="openCreateDialog" :disabled="!currentEmployee">
        <svg viewBox="0 0 24 24"><path d="M12 5v14M5 12h14"/></svg>
        Добавить проводку
      </button>
    </div>

    <div v-if="!currentEmployee" class="card" style="margin-bottom: 20px;">
      <div class="card-body" style="text-align: center; padding: 24px;">
        <div style="color: var(--text-secondary); font-size: 14px;">
          Выберите сотрудника в нижней части боковой панели, чтобы работать с проводками
        </div>
      </div>
    </div>

    <template v-else>
      <div class="card" style="margin-bottom: 20px;">
        <div class="card-body">
          <div class="filter-tabs">
            <button 
              :class="['filter-tab', { active: filter === 'all' }]"
              @click="filter = 'all'; loadTimeEntries()"
            >
              Все время
            </button>
            <button 
              :class="['filter-tab', { active: filter === 'day' }]"
              @click="filter = 'day'"
            >
              Конкретный день
            </button>
            <button 
              :class="['filter-tab', { active: filter === 'month' }]"
              @click="filter = 'month'"
            >
              За месяц
            </button>
          </div>

          <div v-if="filter === 'day'" class="filter-input">
            <div>
              <label class="form-label">Дата</label>
              <input type="date" v-model="selectedDate" class="form-input" @change="loadTimeEntries" />
            </div>
          </div>

          <div v-if="filter === 'month'" class="filter-input">
            <div>
              <label class="form-label">Месяц</label>
              <select v-model="selectedMonth" class="form-select" @change="loadTimeEntries">
                <option v-for="m in 12" :key="m" :value="m">{{ monthNames[m-1] }}</option>
              </select>
            </div>
            <div>
              <label class="form-label">Год</label>
              <select v-model="selectedYear" class="form-select" @change="loadTimeEntries">
                <option v-for="y in years" :key="y" :value="y">{{ y }}</option>
              </select>
            </div>
          </div>
        </div>
      </div>

      <div class="card" style="margin-bottom: 20px;">
        <div class="card-body">
          <div class="section-title">Статус рабочего времени</div>
          <div class="stickers-grid">
            <div class="sticker sticker-yellow">
              <div class="sticker-icon">
                <svg viewBox="0 0 24 24"><path d="M12 9v4M12 17h.01M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"/></svg>
              </div>
              <div class="sticker-info">
                <div class="sticker-hours">Менее 8 часов</div>
                <div class="sticker-status">Недостаточно</div>
              </div>
            </div>
            <div class="sticker sticker-green">
              <div class="sticker-icon">
                <svg viewBox="0 0 24 24"><path d="M20 6L9 17l-5-5"/></svg>
              </div>
              <div class="sticker-info">
                <div class="sticker-hours">Равно 8 часов</div>
                <div class="sticker-status">Норма</div>
              </div>
            </div>
            <div class="sticker sticker-red">
              <div class="sticker-icon">
                <svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="10"/><path d="M12 8v4M12 16h.01"/></svg>
              </div>
              <div class="sticker-info">
                <div class="sticker-hours">Более 8 часов</div>
                <div class="sticker-status">Избыточно</div>
              </div>
            </div>
          </div>

          <div v-if="activeSticker" class="sticker active-sticker" :class="'sticker-' + activeSticker.color">
            <div class="sticker-icon">
              <svg v-if="activeSticker.color === 'yellow'" viewBox="0 0 24 24"><path d="M12 9v4M12 17h.01M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"/></svg>
              <svg v-else-if="activeSticker.color === 'green'" viewBox="0 0 24 24"><path d="M20 6L9 17l-5-5"/></svg>
              <svg v-else viewBox="0 0 24 24"><circle cx="12" cy="12" r="10"/><path d="M12 8v4M12 16h.01"/></svg>
            </div>
            <div class="sticker-info">
              <div class="sticker-hours">{{ activeSticker.hours }} ч</div>
              <div class="sticker-status">{{ activeSticker.status }}</div>
            </div>
          </div>
        </div>
      </div>

      <div class="card">
        <div class="card-header">
          <h2>Список проводок</h2>
          <span class="card-subtitle">{{ timeEntries.length }} записей</span>
        </div>
        
        <table class="data-table">
          <thead>
            <tr>
              <th>Дата</th>
              <th>Задача</th>
              <th>Часы</th>
              <th>Описание</th>
              <th style="width: 100px;"></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="entry in timeEntries" :key="entry.id">
              <td><strong>{{ formatDate(entry.entryDate) }}</strong></td>
              <td>{{ entry.taskName }}</td>
              <td><span class="hours-badge">{{ entry.hours }}ч</span></td>
              <td class="description-cell">{{ entry.description }}</td>
              <td>
                <div class="action-buttons">
                  <button class="btn-icon" @click="editEntry(entry)" title="Изменить">
                    <svg viewBox="0 0 24 24"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
                  </button>
                  <button class="btn-icon danger" @click="deleteEntry(entry)" title="Удалить">
                    <svg viewBox="0 0 24 24"><path d="M3 6h18M19 6v14a2 2 0 01-2 2H7a2 2 0 01-2-2V6m3 0V4a2 2 0 012-2h4a2 2 0 012 2v2"/></svg>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        
        <div v-if="timeEntries.length === 0" class="empty-state">
          <div class="empty-icon">
            <svg viewBox="0 0 24 24"><path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8z"/><path d="M14 2v6h6M16 13H8M16 17H8M10 9H8"/></svg>
          </div>
          <div>Нет проводок</div>
          <div class="empty-hint">Нажмите «Добавить проводку», чтобы создать первую запись</div>
        </div>
      </div>
    </template>

    <el-dialog 
      v-model="showCreateDialog" 
      :title="editingEntry ? 'Редактировать проводку' : 'Добавить проводку'" 
      width="560px"
    >
      <el-form :model="form" label-width="110px">
        <el-form-item label="Задача">
          <el-select 
            v-model="form.taskId" 
            placeholder="Выберите задачу" 
            style="width: 100%"
            :disabled="editingEntry && !canChangeTask"
          >
            <el-option
              v-for="task in activeTasks"
              :key="task.id"
              :label="`${task.projectName} / ${task.name}`"
              :value="task.id"
            />
          </el-select>
          <div v-if="editingEntry && !canChangeTask" style="font-size: 12px; color: var(--warning); margin-top: 4px;">
            Нельзя изменить задачу — исходная задача стала неактивной
          </div>
        </el-form-item>
        <el-form-item label="Дата">
          <el-date-picker
            v-model="form.entryDate"
            type="date"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="Часы">
          <el-input-number v-model="form.hours" :min="0.5" :max="24" :step="0.5" style="width: 100%" />
        </el-form-item>
        <el-form-item label="Описание">
          <el-input v-model="form.description" type="textarea" :rows="3" placeholder="Что было сделано" />
        </el-form-item>
      </el-form>
      <template #footer>
        <button class="btn btn-secondary" @click="showCreateDialog = false">Отмена</button>
        <button class="btn btn-primary" @click="saveEntry">Сохранить</button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { timeEntriesApi, tasksApi } from '../services/api'
import { useEmployeeStore } from '../stores/employee'

const { currentEmployee } = useEmployeeStore()

const timeEntries = ref([])
const activeTasks = ref([])
const allTasks = ref([])
const filter = ref('all')
const selectedDate = ref(new Date().toISOString().split('T')[0])
const selectedMonth = ref(new Date().getMonth() + 1)
const selectedYear = ref(new Date().getFullYear())
const years = ref([2024, 2025, 2026, 2027])
const monthNames = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 
                    'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь']
const showCreateDialog = ref(false)
const editingEntry = ref(null)
const form = ref({ taskId: '', entryDate: '', hours: 8, description: '' })

const activeSticker = computed(() => {
  if (filter.value !== 'day' || !selectedDate.value) return null
  const total = timeEntries.value.reduce((sum, e) => sum + Number(e.hours), 0)
  if (total < 8) return { hours: total, color: 'yellow', status: 'Недостаточно рабочего времени' }
  if (total === 8) return { hours: total, color: 'green', status: 'Норма рабочего времени' }
  return { hours: total, color: 'red', status: 'Превышение нормы рабочего времени' }
})

const canChangeTask = computed(() => {
  if (!editingEntry.value) return true
  const task = allTasks.value.find(t => t.id === editingEntry.value.taskId)
  return task && task.isActive
})

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return date.toLocaleDateString('ru-RU')
}

const loadTimeEntries = async () => {
  if (!currentEmployee.value) return
  
  try {
    let response
    if (filter.value === 'all') {
      response = await timeEntriesApi.getAll()
    } else if (filter.value === 'day') {
      response = await timeEntriesApi.getByDate(selectedDate.value)
    } else {
      response = await timeEntriesApi.getByMonth(selectedYear.value, selectedMonth.value)
    }
    
    timeEntries.value = response.data.filter(e => e.employeeId === currentEmployee.value.id)
  } catch (error) {
    ElMessage.error('Ошибка загрузки: ' + error.message)
  }
}

const loadActiveTasks = async () => {
  try {
    const response = await tasksApi.getActive()
    activeTasks.value = response.data
  } catch (error) {
    console.error(error)
  }
}

const loadAllTasks = async () => {
  try {
    const response = await tasksApi.getAll()
    allTasks.value = response.data
  } catch (error) {
    console.error(error)
  }
}

const openCreateDialog = () => {
  editingEntry.value = null
  form.value = { taskId: '', entryDate: new Date().toISOString().split('T')[0], hours: 8, description: '' }
  showCreateDialog.value = true
}

const saveEntry = async () => {
  try {
    const payload = {
      ...form.value,
      employeeId: currentEmployee.value.id,
    }
    
    if (editingEntry.value) {
      await timeEntriesApi.update(editingEntry.value.id, payload)
      ElMessage.success('Проводка обновлена')
    } else {
      await timeEntriesApi.create(payload)
      ElMessage.success('Проводка создана')
    }
    showCreateDialog.value = false
    await loadTimeEntries()
  } catch (error) {
    ElMessage.error('Ошибка: ' + (error.response?.data?.message || error.message))
  }
}

const editEntry = (entry) => {
  editingEntry.value = entry
  form.value = {
    taskId: entry.taskId,
    entryDate: entry.entryDate,
    hours: entry.hours,
    description: entry.description,
  }
  showCreateDialog.value = true
}

const deleteEntry = async (entry) => {
  try {
    await ElMessageBox.confirm('Удалить проводку?', 'Подтверждение', {
      confirmButtonText: 'Удалить',
      cancelButtonText: 'Отмена',
      type: 'warning'
    })
    await timeEntriesApi.delete(entry.id)
    ElMessage.success('Проводка удалена')
    await loadTimeEntries()
  } catch (error) {
    if (error !== 'cancel') ElMessage.error('Ошибка: ' + error.message)
  }
}

watch(currentEmployee, () => {
  loadTimeEntries()
})

onMounted(() => {
  loadTimeEntries()
  loadActiveTasks()
  loadAllTasks()
})
</script>