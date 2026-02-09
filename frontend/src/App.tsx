import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import HomePage from './pages/HomePage';
import DashboardPage from './pages/DashboardPage';
import EmailDesignPanelAdvancedPage from './pages/EmailDesignPanelAdvancedPage';
import EmailDesignVisualEditorPage from './pages/EmailDesignVisualEditorPage';


function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/email-advanced-editor" element={<EmailDesignPanelAdvancedPage />} />
        <Route path="/email-visual-editor" element={<EmailDesignVisualEditorPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/home" element={<HomePage />} />
        <Route path="/" element={<Navigate to="/home" replace />} />
      </Routes>
    </BrowserRouter>
  )
}
export default App
