import { createRouter, createWebHistory } from 'vue-router'
import ProjectsView from '../views/ProjectsView.vue'
import TasksView from '../views/TasksView.vue'
import TimeEntriesView from '../views/TimeEntriesView.vue'

const routes = [
  { 
    path: '/', 
    redirect: '/projects' 
  },
  { 
    path: '/projects', 
    name: 'Projects', 
    component: ProjectsView 
  },
  { 
    path: '/tasks', 
    name: 'Tasks', 
    component: TasksView 
  },
  { 
    path: '/time-entries', 
    name: 'TimeEntries', 
    component: TimeEntriesView 
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router