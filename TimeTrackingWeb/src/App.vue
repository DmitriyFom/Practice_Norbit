<template>
  <div class="app-layout">
    <aside class="sidebar">
      <div class="sidebar-header">
        <div class="logo">
          <div class="logo-mark">TT</div>
          <div class="logo-text">TimeTrack</div>
        </div>
      </div>

      <nav class="sidebar-nav">
        <div class="nav-section">
          <div class="nav-section-title">Учёт рабочего времени</div>
          <router-link 
            v-for="item in menuItems" 
            :key="item.path" 
            :to="item.path"
            class="nav-item"
            :class="{ active: currentRoute === item.path }"
          >
            <span class="nav-icon">
              <svg viewBox="0 0 24 24"><path :d="item.icon"/></svg>
            </span>
            <span>{{ item.label }}</span>
          </router-link>
        </div>
      </nav>

      <div class="sidebar-footer">
        <button class="theme-toggle" @click="toggleTheme">
          <span class="theme-toggle-icon">
            <svg v-if="isDark" viewBox="0 0 24 24">
              <circle cx="12" cy="12" r="4"/>
              <path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M4.93 19.07l1.41-1.41M17.66 6.34l1.41-1.41"/>
            </svg>
            <svg v-else viewBox="0 0 24 24">
              <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/>
            </svg>
          </span>
          <span>{{ isDark ? 'Светлая тема' : 'Тёмная тема' }}</span>
        </button>

        <div class="employee-section">
          <div class="employee-header">
            <span class="employee-label">Текущий сотрудник</span>
            <button class="add-employee-btn" @click="showAddEmployeeDialog = true" title="Добавить сотрудника">
              <svg viewBox="0 0 24 24">
                <path d="M12 5v14M5 12h14"/>
              </svg>
            </button>
          </div>
          
          <el-popover placement="top-start" :width="280" trigger="click">
            <template #reference>
              <div class="employee-selector">
                <div class="employee-current">
                  <div class="employee-avatar">
                    {{ currentEmployee ? getInitials(currentEmployee) : '?' }}
                  </div>
                  <div class="employee-info">
                    <div class="employee-name">
                      {{ currentEmployee ? `${currentEmployee.firstName} ${currentEmployee.lastName}` : 'Не выбран' }}
                    </div>
                    <div class="employee-email">
                      {{ currentEmployee?.email || '—' }}
                    </div>
                  </div>
                  <div class="employee-change">
                    <svg viewBox="0 0 24 24"><path d="M8 9l4-4 4 4M16 15l-4 4-4-4"/></svg>
                  </div>
                </div>
              </div>
            </template>

            <div class="employee-popover">
              <div 
                v-for="emp in employees" 
                :key="emp.id"
                class="employee-option"
                :class="{ selected: currentEmployee?.id === emp.id }"
                @click="selectEmployee(emp)"
              >
                <div class="employee-avatar">{{ getInitials(emp) }}</div>
                <div class="employee-option-info">
                  <div class="employee-option-name">{{ emp.firstName }} {{ emp.lastName }}</div>
                  <div class="employee-option-email">{{ emp.email }}</div>
                </div>
              </div>
              <div v-if="employees.length === 0" style="padding: 12px; color: var(--text-muted); text-align: center; font-size: 13px;">
                Нет сотрудников
              </div>
            </div>
          </el-popover>
        </div>
      </div>
    </aside>

    <main class="main-content">
      <div class="content-wrapper">
        <router-view />
      </div>
    </main>

    <el-dialog v-model="showAddEmployeeDialog" title="Добавить сотрудника" width="480px">
      <el-form :model="employeeForm" label-width="100px">
        <el-form-item label="Имя">
          <el-input v-model="employeeForm.firstName" placeholder="Иван" />
        </el-form-item>
        <el-form-item label="Фамилия">
          <el-input v-model="employeeForm.lastName" placeholder="Петров" />
        </el-form-item>
        <el-form-item label="Email">
          <el-input v-model="employeeForm.email" placeholder="ivan@mail.ru" />
        </el-form-item>
      </el-form>
      <template #footer>
        <button class="btn btn-secondary" @click="showAddEmployeeDialog = false">Отмена</button>
        <button class="btn btn-primary" @click="addEmployee">Добавить</button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { useEmployeeStore } from './stores/employee'
import { useTheme } from './composables/useTheme'
import { api } from './services/api'

const route = useRoute()
const currentRoute = computed(() => route.path)
const { currentEmployee, employees, loadEmployees, refreshEmployees, setEmployee } = useEmployeeStore()
const { isDark, toggleTheme } = useTheme()

const menuItems = [
  { path: '/projects', label: 'Проекты', icon: 'M3 7h18M3 12h18M3 17h18' },
  { path: '/tasks', label: 'Задачи', icon: 'M9 11l3 3L22 4M21 12v7a2 2 0 01-2 2H5a2 2 0 01-2-2V5a2 2 0 012-2h11' },
  { path: '/time-entries', label: 'Проводки', icon: 'M12 8v4l3 3M12 3a9 9 0 100 18 9 9 0 000-18z' },
]

const getInitials = (emp) => {
  if (!emp) return '?'
  return `${emp.firstName?.[0] || ''}${emp.lastName?.[0] || ''}`.toUpperCase()
}

const selectEmployee = (emp) => setEmployee(emp)

const showAddEmployeeDialog = ref(false)
const employeeForm = ref({ firstName: '', lastName: '', email: '' })

const addEmployee = async () => {
  if (!employeeForm.value.firstName || !employeeForm.value.lastName || !employeeForm.value.email) {
    ElMessage.warning('Заполните все поля')
    return
  }
  
  try {
    const response = await api.post('/Employees', employeeForm.value)
    const newEmployee = response.data
    
    ElMessage.success('Сотрудник добавлен')
    showAddEmployeeDialog.value = false
    employeeForm.value = { firstName: '', lastName: '', email: '' }
    await refreshEmployees()
    setEmployee(newEmployee)
  } catch (error) {
    ElMessage.error('Ошибка: ' + (error.response?.data?.message || error.message))
  }
}

onMounted(() => loadEmployees())
</script>

<style scoped>
.employee-section {
  margin-top: 8px;
}

.employee-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.add-employee-btn {
  width: 24px;
  height: 24px;
  border-radius: var(--radius);
  background: transparent;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  transition: all 0.15s;
}

.add-employee-btn:hover {
  background: var(--bg-muted);
  color: var(--accent);
}

.add-employee-btn svg {
  width: 14px;
  height: 14px;
  stroke: currentColor;
  stroke-width: 2;
  fill: none;
  stroke-linecap: round;
  stroke-linejoin: round;
}
</style>