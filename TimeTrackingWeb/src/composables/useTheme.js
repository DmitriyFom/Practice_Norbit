import { ref, watch, onMounted } from 'vue'

const THEME_KEY = 'timeTrackingTheme'

const getInitialTheme = () => {
  const saved = localStorage.getItem(THEME_KEY)
  if (saved) return saved
  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
}

const currentTheme = ref(getInitialTheme())

export function useTheme() {
  const applyTheme = (theme) => {
    document.documentElement.setAttribute('data-theme', theme)
    localStorage.setItem(THEME_KEY, theme)
    currentTheme.value = theme
  }

  const toggleTheme = () => {
    applyTheme(currentTheme.value === 'dark' ? 'light' : 'dark')
  }

  const setTheme = (theme) => {
    applyTheme(theme)
  }

  const isDark = computed(() => currentTheme.value === 'dark')

  return {
    currentTheme,
    isDark,
    toggleTheme,
    setTheme,
  }
}

applyThemeOnLoad()

function applyThemeOnLoad() {
  const theme = getInitialTheme()
  document.documentElement.setAttribute('data-theme', theme)
}
import { computed } from 'vue'