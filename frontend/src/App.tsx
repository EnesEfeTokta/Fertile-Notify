import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import UnsubscribePage from './pages/UnsubscribePage';
import RegisterPage from './pages/RegisterPage';
import HomePage from './pages/HomePage';
import DashboardPage from './pages/DashboardPage';
import EmailDesignAdvancedPanelPage from './pages/EmailDesignAdvancedPanelPage';
import EmailDesignVisualPanelPage from './pages/EmailDesignVisualPanelPage';
import PricingPlanPage from './pages/PricingPlanPage';
import ChannelDesignPanelPage from './pages/ChannelDesignPanelPage';
import TemplatesPage from './pages/TemplatesPage';
import StatisticsPage from './pages/StatisticsPage';
import AccountPage from './pages/AccountPage';
import ApiKeysPage from './pages/ApiKeysPage';
import BlacklistPage from './pages/BlacklistPage';
import BuyCreditsPage from './pages/BuyCreditsPage';
import LogsPage from './pages/LogsPage';
import WorkflowPage from './pages/WorkflowPage';

// Info Pages
import AboutUsPage from './pages/info/AboutUsPage';
import ContactPage from './pages/info/ContactPage';
import PrivacyPage from './pages/info/PrivacyPage';
import DocumentationPage from './pages/info/DocumentationPage';
import ChangelogPage from './pages/info/ChangelogPage';
import ApiReferencePage from './pages/info/ApiReferencePage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/email-advanced-editor" element={<EmailDesignAdvancedPanelPage />} />
        <Route path="/email-visual-editor" element={<EmailDesignVisualPanelPage />} />
        <Route path="/channel-editor" element={<ChannelDesignPanelPage />} />
        <Route path="/templates" element={<TemplatesPage />} />
        <Route path="/logs" element={<LogsPage />} />
        <Route path="/workflows" element={<WorkflowPage />} />
        <Route path="/statistics" element={<StatisticsPage />} />
        <Route path="/account" element={<AccountPage />} />
        <Route path="/api-keys" element={<ApiKeysPage />} />
        <Route path="/blacklist" element={<BlacklistPage />} />
        <Route path="/unsubscribe" element={<UnsubscribePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/home" element={<HomePage />} />
        <Route path="/pricing" element={<PricingPlanPage />} />
        <Route path="/buy-credits" element={<BuyCreditsPage />} />

        {/* Info Routes */}
        <Route path="/about" element={<AboutUsPage />} />
        <Route path="/contact" element={<ContactPage />} />
        <Route path="/privacy" element={<PrivacyPage />} />
        <Route path="/documentation" element={<DocumentationPage />} />
        <Route path="/changelog" element={<ChangelogPage />} />
        <Route path="/api-reference" element={<ApiReferencePage />} />

        <Route path="/" element={<Navigate to="/home" replace />} />
      </Routes>
    </BrowserRouter>
  )
}
export default App
