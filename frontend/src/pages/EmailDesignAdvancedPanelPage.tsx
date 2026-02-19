import { useMemo, useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import mjml2html from 'mjml-browser';
import Editor from '@monaco-editor/react';
import type { CreateOrUpdateCustom } from '../types/template';
import { templateSevice } from '../api/templateSevice';

const defaultMjml = `<mjml>
  <mj-body background-color="#f0f2f5">
    <mj-section background-color="white" padding="40px">
      <mj-column>
        <mj-text font-size="24px" color="#1f2937" align="center">
          Hello {Name}!
        </mj-text>
        <mj-text font-size="16px" color="#4b5563" align="center">
          Welcome to our service. Your email is {Email}.
        </mj-text>
        <mj-button background-color="#3b82f6" href="{ActionUrl}">
          Get Started
        </mj-button>
      </mj-column>
    </mj-section>
  </mj-body>
</mjml>`;

// System-defined event types
const EVENT_TYPES = [
  { value: 'SubscriberRegistered', label: 'Subscriber Registered', icon: '👋' },
  { value: 'PasswordReset', label: 'Password Reset', icon: '🔐' },
  { value: 'EmailVerified', label: 'Email Verified', icon: '🛒' },
  { value: 'LoginAlert', label: 'Login Alert', icon: '🔔' },
  { value: 'AccountLocked', label: 'Account Locked', icon: '🔒' },
  { value: 'OrderCreated', label: 'Order Created', icon: '📦' },
  { value: 'OrderShipped', label: 'Order Shipped', icon: '🚚' },
  { value: 'OrderDelivered', label: 'Order Delivered', icon: '✅' },
  { value: 'OrderCancelled', label: 'Order Cancelled', icon: '❌' },
  { value: 'PaymentFailed', label: 'Payment Failed', icon: '💰' },
  { value: 'SubscriptionRenewed', label: 'Subscription Renewed', icon: '🔄' },
  { value: 'Campaign', label: 'Campaign', icon: '📢' },
  { value: 'MonthlyNewsletter', label: 'Monthly Newsletter', icon: '📰' },
  { value: 'SupportTicketUpdated', label: 'Support Ticket Updated', icon: '🎫' },
];

export default function EmailDesignAdvancedPanelPage() {
  const [mjmlCode, setMjmlCode] = useState(defaultMjml);
  const [emailSubject, setEmailSubject] = useState('');
  const [activeTab, setActiveTab] = useState<'editor' | 'preview'>('editor');
  const [showSaveModal, setShowSaveModal] = useState(false);
  const [templateName, setTemplateName] = useState('');
  const [templateDescription, setTemplateDescription] = useState('');
  const [selectedEventType, setSelectedEventType] = useState('');
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    // Check for template data in location state
    if (location.state && location.state.template) {
      const { template } = location.state;
      setEmailSubject(template.subject || '');
      setTemplateName(template.name || '');
      setTemplateDescription(template.description || '');
      setSelectedEventType(template.eventType || '');

      // Load template body if available, otherwise default
      setMjmlCode(template.body || defaultMjml);
    }
  }, [location.state]);

  const { html: htmlPreview, hasError, errorMsg } = useMemo(() => {
    try {
      const { html, errors } = mjml2html(mjmlCode);
      if (errors && errors.length > 0) {
        return { html: html || '', hasError: true, errorMsg: (errors[0] as { message: string }).message };
      }
      return { html, hasError: false, errorMsg: '' };
    } catch (e) {
      return { html: '', hasError: true, errorMsg: (e as Error).message };
    }
  }, [mjmlCode]);

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const request: CreateOrUpdateCustom = {
        name: templateName,
        description: templateDescription,
        eventType: selectedEventType,
        channel: 'email',
        subjectTemplate: emailSubject,
        bodyTemplate: mjmlCode
      };
      await templateSevice.createOrUpdateTemplate(request);
      navigate("/templates");
    } catch (error) {
      alert("Template registration failed. Please try again.");
      console.error("Template registration error:", error);
    }
    setShowSaveModal(false);
    setTemplateName('');
    setTemplateDescription('');
    setSelectedEventType('');
  };

  return (
    <div className="flex flex-col h-screen overflow-hidden bg-primary text-primary">
      {/* Save Modal */}
      {showSaveModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/80 backdrop-blur-sm">
          <div className="w-full max-w-md card-elevated p-6">
            <h2 className="text-xl font-semibold text-primary mb-4">Save Template</h2>

            <div className="space-y-4">
              {/* Event Type Selection */}
              <div>
                <label className="block text-sm text-secondary mb-2">Event Type</label>
                <div className="relative">
                  <select
                    value={selectedEventType}
                    onChange={(e) => setSelectedEventType(e.target.value)}
                    className="input-modern appearance-none cursor-pointer"
                  >
                    <option value="" disabled>Select an event type...</option>
                    {EVENT_TYPES.map((event) => (
                      <option key={event.value} value={event.value}>
                        {event.icon} {event.label}
                      </option>
                    ))}
                  </select>
                  <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
                    <svg className="w-4 h-4 text-secondary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                    </svg>
                  </div>
                </div>
              </div>

              <div>
                <label className="block text-sm text-secondary mb-2">Template Name</label>
                <input
                  type="text"
                  value={templateName}
                  onChange={(e) => setTemplateName(e.target.value)}
                  placeholder="Enter template name..."
                  className="input-modern"
                />
              </div>

              <div>
                <label className="block text-sm text-secondary mb-2">Description</label>
                <textarea
                  value={templateDescription}
                  onChange={(e) => setTemplateDescription(e.target.value)}
                  placeholder="Enter description..."
                  rows={2}
                  className="input-modern resize-none"
                />
              </div>
            </div>

            <div className="flex justify-end gap-3 mt-6">
              <button
                onClick={() => setShowSaveModal(false)}
                className="btn-secondary text-sm"
              >
                Cancel
              </button>
              <button
                onClick={handleSave}
                disabled={!templateName.trim() || !selectedEventType}
                className="btn-primary text-sm disabled:opacity-50"
              >
                Save
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Header */}
      <header className="flex items-center justify-between px-6 py-4 bg-secondary border-b border-primary">
        <div className="flex items-center gap-4">
          <button
            onClick={() => navigate('/dashboard')}
            className="p-2 rounded-md bg-tertiary hover:bg-elevated text-secondary hover:text-primary transition-colors"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
            </svg>
          </button>

          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-md bg-primary-500 flex items-center justify-center">
              <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4" />
              </svg>
            </div>
            <div>
              <h1 className="text-base font-semibold text-primary">Code Editor</h1>
              <p className="text-xs text-secondary">MJML</p>
            </div>
          </div>
        </div>

        <div className="flex items-center gap-3">
          {hasError && (
            <div className="px-3 py-1.5 bg-red-500/10 border border-red-500/20 rounded-md text-red-400 text-xs">
              Error
            </div>
          )}

          <button
            onClick={() => setShowSaveModal(true)}
            className="btn-primary text-sm"
          >
            Save
          </button>
        </div>
      </header>

      {/* Email Subject Bar */}
      <div className="px-6 py-3 bg-secondary border-b border-primary flex items-center gap-4">
        <label className="text-sm text-secondary whitespace-nowrap">Subject:</label>
        <input
          type="text"
          value={emailSubject}
          onChange={(e) => setEmailSubject(e.target.value)}
          placeholder="Enter email subject... (use {Variable} for dynamic content)"
          className="flex-1 input-modern text-sm"
        />
      </div>

      {/* Info Banner */}
      <div className="px-6 py-2 bg-primary-500/5 border-b border-primary-500/10 flex items-center gap-2">
        <svg className="w-4 h-4 text-primary-400 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <p className="text-xs text-secondary">
          <span className="font-medium">Tip:</span> Use <code className="px-1 py-0.5 bg-tertiary rounded text-primary-400">{'{Variable}'}</code> syntax for dynamic data. Example: <code className="px-1 py-0.5 bg-tertiary rounded text-primary-400">{'{Name}'}</code>, <code className="px-1 py-0.5 bg-tertiary rounded text-primary-400">{'{Email}'}</code>
        </p>
      </div>

      {/* Mobile Tabs */}
      <div className="flex md:hidden border-b border-primary">
        {(['editor', 'preview'] as const).map((tab) => (
          <button
            key={tab}
            onClick={() => setActiveTab(tab)}
            className={`flex-1 py-3 text-xs font-medium ${activeTab === tab ? 'text-primary-500 border-b-2 border-primary-500' : 'text-tertiary'}`}
          >
            {tab}
          </button>
        ))}
      </div>

      {/* Main Content */}
      <div className="flex flex-1 overflow-hidden">
        {/* Editor */}
        <div className={`flex flex-col flex-1 ${activeTab === 'editor' ? 'flex' : 'hidden md:flex'}`}>
          <div className="px-4 py-2 bg-secondary border-b border-primary flex items-center gap-2">
            <span className="w-2 h-2 rounded-full bg-primary-500"></span>
            <span className="text-xs text-secondary font-mono">template.mjml</span>
          </div>
          <div className="flex-1 relative">
            <Editor
              height="100%"
              defaultLanguage="xml"
              value={mjmlCode}
              onChange={(value) => setMjmlCode(value || '')}
              theme="vs-dark"
              options={{
                fontSize: 14,
                minimap: { enabled: false },
                padding: { top: 16 },
                scrollBeyondLastLine: false,
                wordWrap: 'on',
              }}
            />
            {hasError && (
              <div className="absolute bottom-4 left-4 right-4 bg-red-500/10 border border-red-500/20 p-3 rounded-md">
                <p className="text-xs text-red-300 font-mono">{errorMsg}</p>
              </div>
            )}
          </div>
        </div>

        {/* Preview */}
        <div className={`flex flex-col flex-1 border-l border-primary ${activeTab === 'preview' ? 'flex' : 'hidden md:flex'}`}>
          <div className="px-4 py-2 bg-secondary border-b border-primary flex items-center gap-2">
            <span className="w-2 h-2 rounded-full bg-green-400"></span>
            <span className="text-xs text-secondary font-mono">preview</span>
          </div>
          <div className="flex-1 overflow-auto p-6 bg-black">
            <div className="max-w-[600px] mx-auto h-full bg-white rounded-lg overflow-hidden shadow-xl">
              <iframe
                srcDoc={htmlPreview}
                title="Preview"
                className="w-full h-full border-0"
                sandbox="allow-same-origin allow-scripts"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
