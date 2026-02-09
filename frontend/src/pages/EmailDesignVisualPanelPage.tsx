import { useEffect, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import grapesjs, { type Editor as GjsEditor } from 'grapesjs';
import gjsMjml from 'grapesjs-mjml';
import 'grapesjs/dist/css/grapes.min.css';

const defaultMjml = `
<mjml>
  <mj-body background-color="#f0f2f5">
    <mj-section background-color="#ffffff" padding="40px">
      <mj-column>
        <mj-text font-size="24px" color="#1f2937" align="center">Hello {Name}!</mj-text>
        <mj-text font-size="16px" color="#4b5563" align="center">
          Welcome to our service.
        </mj-text>
        <mj-button background-color="#8b5cf6" href="#">Get Started</mj-button>
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
    editor.setComponents(defaultMjml);

    setTimeout(() => {
      if (editor) editor.refresh();
    }, 200);

    return () => editor.destroy();
  }, []);

  const handleSave = () => {
    if (gjsRef.current) {
      const mjml = gjsRef.current.getHtml();
      console.log('Saving template:', { templateName, templateDescription, emailSubject, selectedEventType, mjml });
      // TODO: API call
    }
    setShowSaveModal(false);
    setTemplateName('');
    setTemplateDescription('');
    setSelectedEventType('');
  };

  return (
    <div className="flex flex-col h-screen overflow-hidden bg-gradient-to-br from-gray-900 via-purple-900 to-gray-900 text-white">
      {/* GrapesJS Styles */}
      <style dangerouslySetInnerHTML={{
        __html: `
        .gjs-cv-canvas {
          background-color: transparent !important;
          background-image: radial-gradient(#374151 1px, transparent 1px) !important;
          background-size: 20px 20px !important;
        }
        .gjs-one-bg { background-color: transparent !important; }
        .gjs-two-color { color: #9ca3af !important; }
        .gjs-block {
          background: #1f2937 !important;
          border: 1px solid #374151 !important;
          border-radius: 8px !important;
          color: #e5e7eb !important;
          transition: all 0.2s !important;
          padding: 10px !important;
        }
        .gjs-block:hover {
          border-color: #8b5cf6 !important;
          background: #272f42 !important;
        }
        .gjs-block-label { font-size: 0.7rem !important; text-transform: uppercase; }
        .gjs-field { background-color: #111827 !important; border: 1px solid #374151 !important; border-radius: 4px !important; color: white !important; }
        .gjs-category-title, .gjs-sm-sector-title {
          background-color: #111827 !important;
          border-bottom: 1px solid #374151 !important;
          color: #e5e7eb !important;
          font-size: 0.7rem !important;
          text-transform: uppercase !important;
          padding: 12px !important;
        }
        .gjs-sm-property .gjs-sm-label { color: #9ca3af !important; font-size: 0.7rem !important; }
        ::-webkit-scrollbar { width: 6px; }
        ::-webkit-scrollbar-track { background: #111827; }
        ::-webkit-scrollbar-thumb { background: #374151; border-radius: 3px; }
      `}} />

      {/* Save Modal */}
      {showSaveModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm">
          <div className="w-full max-w-md bg-gray-900 border border-purple-500/30 rounded-xl shadow-2xl p-6">
            <h2 className="text-xl font-bold text-white mb-4">Save Template</h2>

            <div className="space-y-4">
              {/* Event Type Selection */}
              <div>
                <label className="block text-sm text-gray-400 mb-2">Event Type</label>
                <div className="relative">
                  <select
                    value={selectedEventType}
                    onChange={(e) => setSelectedEventType(e.target.value)}
                    className="w-full px-4 py-2.5 bg-gray-800 border border-gray-700 rounded-lg text-white appearance-none focus:outline-none focus:border-purple-500 cursor-pointer"
                  >
                    <option value="" disabled>Select an event type...</option>
                    {EVENT_TYPES.map((event) => (
                      <option key={event.value} value={event.value}>
                        {event.icon} {event.label}
                      </option>
                    ))}
                  </select>
                  <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none">
                    <svg className="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                    </svg>
                  </div>
                </div>
              </div>

              <div>
                <label className="block text-sm text-gray-400 mb-1">Template Name</label>
                <input
                  type="text"
                  value={templateName}
                  onChange={(e) => setTemplateName(e.target.value)}
                  placeholder="Enter template name..."
                  className="w-full px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-purple-500"
                />
              </div>

              <div>
                <label className="block text-sm text-gray-400 mb-1">Description</label>
                <textarea
                  value={templateDescription}
                  onChange={(e) => setTemplateDescription(e.target.value)}
                  placeholder="Enter description..."
                  rows={2}
                  className="w-full px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-purple-500 resize-none"
                />
              </div>
            </div>

            <div className="flex justify-end gap-3 mt-6">
              <button
                onClick={() => setShowSaveModal(false)}
                className="px-4 py-2 text-sm text-gray-400 hover:text-white transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleSave}
                disabled={!templateName.trim() || !selectedEventType}
                className="px-6 py-2 bg-gradient-to-r from-purple-600 to-pink-600 text-white rounded-lg font-medium disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Save
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Header */}
      <header className="flex items-center justify-between px-6 py-4 bg-gray-900/80 backdrop-blur-md border-b border-purple-500/30 shrink-0">
        <div className="flex items-center gap-4">
          <button
            onClick={() => navigate('/dashboard')}
            className="p-2 rounded-lg bg-gray-800 hover:bg-gray-700 text-gray-400 hover:text-white transition-colors"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
            </svg>
          </button>

          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-lg bg-gradient-to-br from-pink-500 to-purple-600 flex items-center justify-center">
              <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
              </svg>
            </div>
            <div>
              <h1 className="text-lg font-bold text-white">Visual Editor</h1>
              <p className="text-xs text-gray-400">Drag & Drop</p>
            </div>
          </div>
        </div>

        <button
          onClick={() => setShowSaveModal(true)}
          className="px-5 py-2 bg-gradient-to-r from-purple-600 to-pink-600 text-white rounded-lg font-medium text-sm hover:opacity-90 transition-opacity"
        >
          Save
        </button>
      </header>

      {/* Email Subject Bar */}
      <div className="px-6 py-3 bg-gray-900/50 border-b border-gray-800 flex items-center gap-4 shrink-0">
        <label className="text-sm text-gray-400 whitespace-nowrap">Subject:</label>
        <input
          type="text"
          value={emailSubject}
          onChange={(e) => setEmailSubject(e.target.value)}
          placeholder="Enter email subject... (use {Variable} for dynamic content)"
          className="flex-1 px-4 py-2 bg-gray-800 border border-gray-700 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-purple-500 text-sm"
        />
      </div>

      {/* Info Banner */}
      <div className="px-6 py-2 bg-purple-900/20 border-b border-purple-500/20 flex items-center gap-2 shrink-0">
        <svg className="w-4 h-4 text-purple-400 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <p className="text-xs text-purple-300">
          <span className="font-medium">Tip:</span> Use <code className="px-1 py-0.5 bg-purple-500/20 rounded text-purple-200">{'{Variable}'}</code> syntax for dynamic data. Example: <code className="px-1 py-0.5 bg-purple-500/20 rounded text-purple-200">{'{Name}'}</code>, <code className="px-1 py-0.5 bg-purple-500/20 rounded text-purple-200">{'{Email}'}</code>
        </p>
      </div>

      {/* Main Content */}
      <div className="flex flex-1 overflow-hidden">
        {/* Left Sidebar - Blocks */}
        <div className="w-64 bg-gray-900 border-r border-gray-800 flex flex-col shrink-0">
          <div className="px-4 py-3 border-b border-gray-800 text-xs font-medium text-gray-400 uppercase">
            Blocks
          </div>
          <div id="blocks-container" className="flex-1 overflow-y-auto p-3"></div>
        </div>

        {/* Canvas */}
        <div className="flex-1 overflow-hidden bg-black/50">
          <div className="h-full" ref={editorRef}></div>
        </div>

        {/* Styles Sidebar */}
        <div className="w-72 bg-gray-900 border-l border-gray-800 flex flex-col shrink-0">
          <div className="px-4 py-3 border-b border-gray-800 text-xs font-medium text-gray-400 uppercase">
            Settings
          </div>
          <div className="flex-1 overflow-y-auto">
            <div id="styles-container"></div>
            <div id="traits-container" className="border-t border-gray-800"></div>
          </div>
        </div>
      </div>
    </div>
  );
}
