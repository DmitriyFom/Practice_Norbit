<template>
  <div class="projects-view">
    <div class="page-header">
      <div class="page-title">
        <h1>Проекты</h1>
        <p class="page-subtitle">Управление проектами компании</p>
      </div>
      <button class="btn btn-primary" @click="openCreateDialog">
        <svg viewBox="0 0 24 24"><path d="M12 5v14M5 12h14"/></svg>
        Добавить проект
      </button>
    </div>

    <div class="card">
      <div class="card-header">
        <h2>Список проектов</h2>
        <span class="card-subtitle">{{ projects.length }} проектов</span>
      </div>
      
      <table class="data-table">
        <thead>
          <tr>
            <th>Название</th>
            <th>Код</th>
            <th>Статус</th>
            <th>Создан</th>
            <th style="width: 100px;"></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="project in projects" :key="project.id">
            <td><strong>{{ project.name }}</strong></td>
            <td>
              <code class="code-badge">{{ project.code }}</code>
            </td>
            <td>
              <span :class="['badge', project.isActive ? 'badge-active' : 'badge-inactive']">
                <span class="badge-dot"></span>
                {{ project.isActive ? 'Активен' : 'Неактивен' }}
              </span>
            </td>
            <td>{{ formatDate(project.createdAt) }}</td>
            <td>
              <div class="action-buttons">
                <button class="btn-icon" @click="editProject(project)" title="Изменить">
                  <svg viewBox="0 0 24 24"><path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>
                </button>
                <button class="btn-icon danger" @click="deleteProject(project)" title="Удалить">
                  <svg viewBox="0 0 24 24"><path d="M3 6h18M19 6v14a2 2 0 01-2 2H7a2 2 0 01-2-2V6m3 0V4a2 2 0 012-2h4a2 2 0 012 2v2"/></svg>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      
      <div v-if="projects.length === 0" class="empty-state">
        <div class="empty-icon">
          <svg viewBox="0 0 24 24"><path d="M22 19a2 2 0 01-2 2H4a2 2 0 01-2-2V5a2 2 0 012-2h5l2 3h9a2 2 0 012 2z"/></svg>
        </div>
        <div>Нет проектов</div>
        <div class="empty-hint">Нажмите «Добавить проект», чтобы создать первый проект</div>
      </div>
    </div>

    <el-dialog 
      v-model="showCreateDialog" 
      :title="editingProject ? 'Редактировать проект' : 'Создать проект'" 
      width="520px"
    >
      <el-form :model="form" label-width="110px">
        <el-form-item label="Название">
          <el-input v-model="form.name" placeholder="Например, Веб-портал" />
        </el-form-item>
        <el-form-item label="Код">
          <el-input v-model="form.code" placeholder="Например, WEB-001" />
        </el-form-item>
        <el-form-item label="Активен">
          <el-switch v-model="form.isActive" />
        </el-form-item>
      </el-form>
      <template #footer>
        <button class="btn btn-secondary" @click="showCreateDialog = false">Отмена</button>
        <button class="btn btn-primary" @click="saveProject">Сохранить</button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { projectsApi } from '../services/api'

const projects = ref([])
const showCreateDialog = ref(false)
const editingProject = ref(null)
const form = ref({ name: '', code: '', isActive: true })

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return date.toLocaleDateString('ru-RU')
}

const loadProjects = async () => {
  try {
    const response = await projectsApi.getAll()
    projects.value = response.data
  } catch (error) {
    ElMessage.error('Ошибка загрузки: ' + error.message)
  }
}

const openCreateDialog = () => {
  editingProject.value = null
  form.value = { name: '', code: '', isActive: true }
  showCreateDialog.value = true
}

const saveProject = async () => {
  try {
    if (editingProject.value) {
      await projectsApi.update(editingProject.value.id, form.value)
      ElMessage.success('Проект обновлён')
    } else {
      await projectsApi.create(form.value)
      ElMessage.success('Проект создан')
    }
    showCreateDialog.value = false
    await loadProjects()
  } catch (error) {
    ElMessage.error('Ошибка: ' + (error.response?.data?.message || error.message))
  }
}

const editProject = (project) => {
  editingProject.value = project
  form.value = { name: project.name, code: project.code, isActive: project.isActive }
  showCreateDialog.value = true
}

const deleteProject = async (project) => {
  try {
    await ElMessageBox.confirm(
      `Удалить проект «${project.name}»?`,
      'Подтверждение',
      {
        confirmButtonText: 'Удалить',
        cancelButtonText: 'Отмена',
        type: 'warning'
      }
    )
    await projectsApi.delete(project.id)
    ElMessage.success('Проект удалён')
    await loadProjects()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('Ошибка: ' + (error.response?.data?.message || error.message))
    }
  }
}

onMounted(loadProjects)
</script>

<style scoped>
.code-badge {
  display: inline-block;
  padding: 2px 8px;
  background: var(--bg-subtle);
  color: var(--text);
  border-radius: 4px;
  font-size: 12.5px;
  font-weight: 600;
  font-family: 'SF Mono', Monaco, 'Cascadia Code', monospace;
  border: 1px solid var(--border);
}
</style>