import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import HomePage from './pages/HomePage';
import DashboardPage from './pages/DashboardPage';
import EmailDesignAdvancedPanelPage from './pages/EmailDesignAdvancedPanelPage';
import EmailDesignVisualPanelPage from './pages/EmailDesignVisualPanelPage';
import PricingPlanPage from './pages/PricingPlanPage';
import SmsDesignPanelPage from './pages/SmsDesignPanelPage';


function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/email-advanced-editor" element={<EmailDesignAdvancedPanelPage />} />
        <Route path="/email-visual-editor" element={<EmailDesignVisualPanelPage />} />
        <Route path="/sms-editor" element={<SmsDesignPanelPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/home" element={<HomePage />} />
        <Route path="/pricing" element={<PricingPlanPage />} />
        <Route path="/" element={<Navigate to="/home" replace />} />
      </Routes>
    </BrowserRouter>
  )
}
export default App
