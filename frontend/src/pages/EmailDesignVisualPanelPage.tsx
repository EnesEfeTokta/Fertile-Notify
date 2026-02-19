import { useEffect, useRef, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import grapesjs, { type Editor as GjsEditor } from 'grapesjs';
import gjsMjml from 'grapesjs-mjml';
import 'grapesjs/dist/css/grapes.min.css';
import type { CreateOrUpdateCustom } from '../types/template';
import { templateSevice } from '../api/templateSevice';

const defaultMjml = `
<mjml>
  <mj-body background-color="#f0f2f5">
    <mj-section background-color="#ffffff" padding="40px">
      <mj-column>
        <mj-text font-size="24px" color="#1f2937" align="center">Hello {Name}!</mj-text>
        <mj-text font-size="16px" color="#4b5563" align="center">
          Welcome to our service.
        </mj-text>
        <mj-button background-color="#3b82f6" href="#">Get Started</mj-button>
      </mj-column>
    </mj-section>
  </mj-body>
</mjml>
`;

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

export default function EmailDesignVisualPanelPage() {
  const editorRef = useRef<HTMLDivElement>(null);
  const gjsRef = useRef<GjsEditor | null>(null);
  const [emailSubject, setEmailSubject] = useState('');
  const [showSaveModal, setShowSaveModal] = useState(false);
  const [templateName, setTemplateName] = useState('');
  const [templateDescription, setTemplateDescription] = useState('');
  const [selectedEventType, setSelectedEventType] = useState('');
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    if (!editorRef.current) return;

    const editor = grapesjs.init({
      container: editorRef.current,
      fromElement: false,
      height: '100%',
      width: 'auto',
      plugins: [gjsMjml],
      pluginsOpts: { 'grapesjs-mjml': {} },
      storageManager: false,
      panels: { defaults: [] },
      blockManager: { appendTo: '#blocks-container' },
      styleManager: { appendTo: '#styles-container' },
      traitManager: { appendTo: '#traits-container' },
      selectorManager: { appendTo: '#styles-container' },
      deviceManager: {
        devices: [
          { name: 'Desktop', width: '' },
          { name: 'Mobile', width: '320px' }
        ]
      }
    });

    gjsRef.current = editor;

    // Check for template data in location state
    if (location.state && location.state.template) {
      const { template } = location.state;
      setEmailSubject(template.subject || '');
      setTemplateName(template.name || '');
      setTemplateDescription(template.description || '');
      setSelectedEventType(template.eventType || '');

      // Load template body if available, otherwise default
      editor.setComponents(template.body || defaultMjml);
    } else {
      editor.setComponents(defaultMjml);
    }

    setTimeout(() => {
      if (editor) editor.refresh();
    }, 200);

    return () => editor.destroy();
  }, [location.state]);

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (gjsRef.current) {
      const mjml = gjsRef.current.getHtml();
      console.log('Saving template:', { templateName, templateDescription, emailSubject, selectedEventType, mjml });
      try {
        const request: CreateOrUpdateCustom = {
          name: templateName,
          description: templateDescription,
          eventType: selectedEventType,
          channel: 'email',
          subjectTemplate: emailSubject,
          bodyTemplate: mjml
        };
        await templateSevice.createOrUpdateTemplate(request);
        navigate("/templates");
      } catch (error) {
        alert("Template registration failed. Please try again.");
        console.error("Template registration error:", error);
      }
    }
    setShowSaveModal(false);
    setTemplateName('');
    setTemplateDescription('');
    setSelectedEventType('');
  };

  return (
    <div className="flex flex-col h-screen overflow-hidden bg-primary text-primary">
      {/* GrapesJS Styles */}
      <style dangerouslySetInnerHTML={{
        __html: `
        .gjs-cv-canvas {
          background-color: transparent !important;
          background-image: radial-gradient(#262626 1px, transparent 1px) !important;
          background-size: 20px 20px !important;
        }
        .gjs-one-bg { background-color: transparent !important; }
        .gjs-two-color { color: #a1a1a1 !important; }
        .gjs-block {
          background: #0a0a0a !important;
          border: 1px solid #262626 !important;
          border-radius: 6px !important;
          color: #ededed !important;
          transition: all 0.2s !important;
          padding: 10px !important;
        }
        .gjs-block:hover {
          border-color: #3b82f6 !important;
          background: #111111 !important;
        }
        .gjs-block-label { font-size: 0.7rem !important; }
        .gjs-field { 
          background-color: #000000 !important; 
          border: 1px solid #262626 !important; 
          border-radius: 4px !important; 
          color: #ededed !important; 
        }
        .gjs-category-title, .gjs-sm-sector-title {
          background-color: #000000 !important;
          border-bottom: 1px solid #262626 !important;
          color: #ededed !important;
          font-size: 0.7rem !important;
          padding: 12px !important;
        }
        .gjs-sm-property .gjs-sm-label { color: #a1a1a1 !important; font-size: 0.7rem !important; }
        ::-webkit-scrollbar { width: 6px; }
        ::-webkit-scrollbar-track { background: #000000; }
        ::-webkit-scrollbar-thumb { background: #262626; border-radius: 3px; }
      `}} />

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
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
            </div>
            <div>
              <h1 className="text-base font-semibold text-primary">Visual Editor</h1>
              <p className="text-xs text-secondary">Drag & Drop</p>
            </div>
          </div>
        </div>

        <button
          onClick={() => setShowSaveModal(true)}
          className="btn-primary text-sm"
        >
          Save
        </button>
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

      {/* Main Content */}
      <div className="flex flex-1 overflow-hidden">
        {/* Left Sidebar - Blocks */}
        <div className="w-64 bg-secondary border-r border-primary flex flex-col">
          <div className="px-4 py-3 border-b border-primary text-xs font-medium text-secondary">
            Blocks
          </div>
          <div id="blocks-container" className="flex-1 overflow-y-auto p-3"></div>
        </div>

        {/* Canvas */}
        <div className="flex-1 overflow-hidden bg-primary">
          <div className="h-full" ref={editorRef}></div>
        </div>

        {/* Styles Sidebar */}
        <div className="w-72 bg-secondary border-l border-primary flex flex-col">
          <div className="px-4 py-3 border-b border-primary text-xs font-medium text-secondary">
            Settings
          </div>
          <div className="flex-1 overflow-y-auto">
            <div id="styles-container"></div>
            <div id="traits-container" className="border-t border-primary"></div>
          </div>
        </div>
      </div>
    </div>
  );
}
