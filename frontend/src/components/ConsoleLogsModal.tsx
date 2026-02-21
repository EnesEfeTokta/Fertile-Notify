import { useState, useEffect } from 'react';
import type { Notification } from '../types/template';
import { templateSevice } from '../api/templateSevice';

interface ConsoleLogsModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export default function ConsoleLogsModal({ isOpen, onClose }: ConsoleLogsModalProps) {
    const [logs, setLogs] = useState<Notification[]>([]);
    const [typedText, setTypedText] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const fullText = "Initializing Fertile-Notify Console Listener... [OK]";

    const fetchLogs = async () => {
        try {
            setIsLoading(true);
            const res = await templateSevice.getNotificationLogs();
            setLogs(res);
        } catch (error) {
            console.error('Failed to fetch logs:', error);
        } finally {
            setIsLoading(false);
        }
    };

    // Mock logs for demonstration
    useEffect(() => {
        if (isOpen) {
            fetchLogs();

            let i = 0;
            const interval = setInterval(() => {
                setTypedText(fullText.slice(0, i));
                i++;
                if (i > fullText.length) clearInterval(interval);
            }, 50);

            return () => clearInterval(interval);
        }
    }, [isOpen]);

    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-[100] flex items-center justify-center bg-black/80 backdrop-blur-md px-4">
            <div
                className="w-full max-w-4xl h-[600px] flex flex-col bg-[#0a0a0b] border border-[#262626] rounded-lg shadow-[0_0_50px_-12px_rgba(0,0,0,0.5)] overflow-hidden animate-slide-up"
                style={{ fontFamily: "'JetBrains Mono', 'Fira Code', 'Courier New', monospace" }}
            >
                {/* Terminal Header */}
                <div className="flex items-center justify-between px-4 py-2.5 bg-[#161617] border-b border-[#262626] select-none">
                    <div className="flex items-center gap-2">
                        <div className="flex gap-1.5">
                            <div onClick={onClose} className="w-3 h-3 rounded-full bg-[#ff5f56] cursor-pointer hover:brightness-110 active:brightness-90 transition-all"></div>
                            <div className="w-3 h-3 rounded-full bg-[#ffbd2e]"></div>
                            <div className="w-3 h-3 rounded-full bg-[#27c93f]"></div>
                        </div>
                        <div className="ml-4 flex items-center gap-2">
                            <svg className="w-4 h-4 text-secondary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 9l3 3-3 3m5 0h3M5 20h14a2 2 0 002-2V6a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                            </svg>
                            <span className="text-[11px] font-bold text-secondary uppercase tracking-widest">fertile@notify: ~/console</span>
                        </div>
                    </div>
                    <div className="flex items-center gap-4">
                        <button
                            onClick={fetchLogs}
                            disabled={isLoading}
                            className={`flex items-center gap-2 px-2 py-1 rounded hover:bg-white/5 transition-colors group ${isLoading ? 'opacity-50 cursor-not-allowed' : ''}`}
                            title="Refresh Logs"
                        >
                            <svg className={`w-3.5 h-3.5 text-secondary group-hover:text-primary ${isLoading ? 'animate-spin' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                            </svg>
                            <span className="text-[10px] font-bold text-secondary group-hover:text-primary uppercase tracking-wider">Refresh</span>
                        </button>
                        <div className="flex items-center gap-1.5 px-2 py-0.5 bg-green-500/10 rounded-full border border-green-500/20">
                            <div className="w-1.5 h-1.5 rounded-full bg-green-500 animate-pulse"></div>
                            <span className="text-[9px] font-bold text-green-500/80 uppercase">Live</span>
                        </div>
                    </div>
                </div>

                {/* Terminal Body */}
                <div className="flex-1 overflow-y-auto p-6 scrollbar-terminal">
                    <div className="space-y-1 mb-6">
                        <div className="flex items-center gap-2 text-[#4ade80]">
                            <span className="opacity-50 text-[13px]">➜</span>
                            <span className="text-[13px] font-bold">{typedText}</span>
                            <span className="w-2 h-4 bg-[#4ade80] animate-pulse"></span>
                        </div>
                    </div>

                    <div className="space-y-3 font-mono">
                        {logs.map((log) => (
                            <div key={log.id} className="group animate-fade-in flex flex-col gap-1 text-[13px] leading-relaxed border-l-2 border-transparent hover:border-[#262626] pl-2 transition-colors">
                                <div className="flex gap-4">
                                    <span className="text-secondary/40 shrink-0 select-none">
                                        [{new Date(log.createdAt).toLocaleTimeString([], { hour12: false })}]
                                    </span>
                                    <div className="flex gap-2">
                                        <span className={`font-bold uppercase shrink-0 transition-colors ${log.event === 'error' ? 'text-red-400' :
                                            log.event === 'warning' ? 'text-yellow-400' :
                                                log.event === 'info' ? 'text-blue-400' :
                                                    'text-green-400'
                                            }`}>
                                            {log.event}
                                        </span>
                                        <span className="text-primary font-bold">
                                            {log.subject}
                                        </span>
                                    </div>
                                </div>
                                <div className="pl-[78px] text-secondary/80 text-[12px] italic">
                                    {log.body}
                                </div>
                            </div>
                        ))}

                        {/* Typing Placeholder */}
                        <div className="flex gap-4 text-[13px] opacity-40">
                            <span className="text-secondary/40 shrink-0">
                                [{new Date().toLocaleTimeString([], { hour12: false })}]
                            </span>
                            <span className="animate-pulse">_ await next event...</span>
                        </div>
                    </div>
                </div>

                {/* Terminal Footer */}
                <div className="px-4 py-2 bg-[#0c0c0d] border-t border-[#262626] flex items-center justify-between">
                    <div className="flex items-center gap-4">
                        <div className="flex items-center gap-2">
                            <span className="text-[10px] text-tertiary">Logs:</span>
                            <span className="text-[10px] font-bold text-primary">{logs.length}</span>
                        </div>
                        <div className="flex items-center gap-2 border-l border-[#262626] pl-4">
                            <span className="text-[10px] text-tertiary">Status:</span>
                            <span className="text-[10px] font-bold text-green-500/80">{isLoading ? 'FETCHING...' : 'ONLINE'}</span>
                        </div>
                    </div>
                    <button
                        onClick={onClose}
                        className="text-[10px] font-bold text-secondary hover:text-primary transition-colors uppercase tracking-widest px-3 py-1 hover:bg-[#1a1a1b] rounded"
                    >
                        Exit (ESC)
                    </button>
                </div>

                <style dangerouslySetInnerHTML={{
                    __html: `
          .scrollbar-terminal::-webkit-scrollbar { width: 8px; }
          .scrollbar-terminal::-webkit-scrollbar-track { background: #0a0a0b; }
          .scrollbar-terminal::-webkit-scrollbar-thumb { 
            background: #1f1f21; 
            border: 2px solid #0a0a0b;
            border-radius: 4px;
          }
          .scrollbar-terminal::-webkit-scrollbar-thumb:hover { background: #262628; }
        `}} />
            </div>
        </div>
    );
}
