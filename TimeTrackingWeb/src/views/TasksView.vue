<template>
  <div class="tasks-view">
    <div class="page-header">
      <div class="page-title">
        <h1>Задачи</h1>
        <p class="page-subtitle">Управление задачами по проектам</p>
      </div>
      <button class="btn btn-primary" @click="openCreateDialog">
        <svg viewBox="0 0 24 24"><path d="M12 5v14M5 12h14"/></svg>
        Добавить задачу
      </button>
    </div>

    <div class="card" style="margin-bottom: 20px;">
      <div class="card-body">
        <div class="filter-row">
          <div class="filter-item">
            <label class="form-label">Фильтр по проекту</label>
            <select v-model="selectedProject" class="form-select" @change="loadTasks">
              <option value="">Все проекты</option>
              <option v-for="project in allProjects" :key="project.id" :value="project.id">
                {{ project.name }}
              </option>
            </select>
          </div>
          <div class="filter-item">
            <label class="form-label">Статус</label>
            <select v-model="statusFilter" class="form-select" @change="loadTasks">
              <option value="">Все</option>
              <option value="active">Только активные</option>
              <option value="inactive">Только неактивные</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <div class="card">
      <div class="card-header">
        <h2>Список задач</h2>
        <span class="card-subtitle">{{ filteredTasks.length }} задач</span>
      </div>
      
      <table class="data-table">
        <thead>
          <tr>
            <th>Название</th>
            <th>Проект</th>
            <th>Статус</th>
            <th>Создана</th>
            <th style="width: 100px;"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="task in filteredTasks" :key="task.id">
            <td><strong>{{ task.name }}</strong></td>
            <td>
              <span class="project-link">{{ task.projectName }}</span>
            </td>
            <td>
              <span :class="['badge', task.isActive ? 'badge-active' : 'badge-inactive']">
                <span class="badge-dot"></span>
                {{ task.isActive ? 'Активна' : 'Неактивна' }}
              </span>
            </td>
            <td>{{ formatDate(task.createdAt) }}</td>
            <td>
              <div class="action-buttons">
                <button class="btn-icon" @click="editTask(task)" title="Изменить">
                  <svg viewBox="0 0 24 24"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
                </button>
                <button class="btn-icon danger" @click="deleteTask(task)" title="Удалить">
                  <svg viewBox="0 0 24 24"><path d="M3 6h18M19 6v14a2 2 0 01-2 2H7a2 2 0 01-2-2V6m3 0V4a2 2 0 012-2h4a2 2 0 012 2v2"/></svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      
      <div v-if="filteredTasks.length === 0" class="empty-state">
        <div class="empty-icon">
          <svg viewBox="0 0 24 24"><path d="M9 11l3 3L22 4"/><path d="M21 12v7a2 2 0 01-2 2H5a2 2 0 01-2-2V5a2 2 0 012-2h11"/></svg>
        </div>
        <div>Нет задач</div>
        <div class="empty-hint">Нажмите «Добавить задачу», чтобы создать первую задачу</div>
      </div>
    </div>

    <el-dialog 
      v-model="showCreateDialog" 
      :title="editingTask ? 'Редактировать задачу' : 'Создать задачу'" 
      width="520px"
    >
      <el-form :model="form" label-width="110px">
        <el-form-item label="Название">
          <el-input v-model="form.name" placeholder="Например, Разработка API" />
        </el-form-item>
        <el-form-item label="Проект">
          <el-select v-model="form.projectId" placeholder="Выберите проект" style="width: 100%">
            <el-option
              v-for="project in activeProjects"
              :key="project.id"
              :label="project.name"
              :value="project.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="Активна">
          <el-switch v-model="form.isActive" />
        </el-form-item>
      </el-form>
      <template #footer>
        <button class="btn btn-secondary" @click="showCreateDialog = false">Отмена</button>
        <button class="btn btn-primary" @click="saveTask">Сохранить</button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { tasksApi, projectsApi } from '../services/api'

const tasks = ref([])
const allProjects = ref([])
const selectedProject = ref('')
const statusFilter = ref('')
const showCreateDialog = ref(false)
const editingTask = ref(null)
const form = ref({ name: '', projectId: '', isActive: true })

const activeProjects = computed(() => allProjects.value.filter(p => p.isActive))

const filteredTasks = computed(() => {
  let result = tasks.value
  
  if (selectedProject.value) {
    result = result.filter(t => t.projectId === selectedProject.value)
  }
  
  if (statusFilter.value === 'active') {
    result = result.filter(t => t.isActive)
  } else if (statusFilter.value === 'inactive') {
    result = result.filter(t => !t.isActive)
  }
  
  return result
})

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return date.toLocaleDateString('ru-RU')
}

const loadTasks = async () => {
  try {
    const response = await tasksApi.getAll()
    tasks.value = response.data
  } catch (error) {
    ElMessage.error('Ошибка загрузки: ' + error.message)
  }
}

const loadProjects = async () => {
  try {
    const response = await projectsApi.getAll()
    allProjects.value = response.data
  } catch (error) {
    console.error(error)
  }
}

const openCreateDialog = () => {
  editingTask.value = null
  form.value = { name: '', projectId: '', isActive: true }
  showCreateDialog.value = true
}

const saveTask = async () => {
  try {
    if (editingTask.value) {
      await tasksApi.update(editingTask.value.id, form.value)
      ElMessage.success('Задача обновлена')
    } else {
      await tasksApi.create(form.value)
      ElMessage.success('Задача создана')
    }
    showCreateDialog.value = false
    await loadTasks()
  } catch (error) {
    ElMessage.error('Ошибка: ' + (error.response?.data?.message || error.message))
  }
}

const editTask = (task) => {
  editingTask.value = task
  form.value = { name: task.name, projectId: task.projectId, isActive: task.isActive }
  showCreateDialog.value = true
}

const deleteTask = async (task) => {
  try {
    await ElMessageBox.confirm(
      `Удалить задачу «${task.name}»?`,
      'Подтверждение',
      {
        confirmButtonText: 'Удалить',
        cancelButtonText: 'Отмена',
        type: 'warning'
      }
    )
    await tasksApi.delete(task.id)
    ElMessage.success('Задача удалена')
    await loadTasks()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('Ошибка: ' + (error.response?.data?.message || error.message))
    }
  }
}

onMounted(() => {
  loadTasks()
  loadProjects()
})
</script>

<style scoped>
.filter-row {
  display: flex;
  gap: 16px;
}

.filter-item {
  flex: 1;
  max-width: 280px;
}

.project-link {
  display: inline-block;
  padding: 2px 10px;
  background: var(--accent-subtle);
  color: var(--accent-text);
  border-radius: 4px;
  font-size: 12.5px;
  font-weight: 550;
}
</style>