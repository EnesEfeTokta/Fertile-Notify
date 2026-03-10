import { useState, useCallback } from "react";

export interface Toast {
    id: number;
    message: string;
    type: "success" | "error" | "info";
}

let toastId = 0;

// A simple module-level hook + renderer pair
// Usage: const { showToast, ToastContainer } = useToast();
export function useToast() {
    const [toasts, setToasts] = useState<Toast[]>([]);

    const showToast = useCallback((message: string, type: Toast["type"] = "success") => {
        const id = ++toastId;
        setToasts(prev => [...prev, { id, message, type }]);
        setTimeout(() => {
            setToasts(prev => prev.filter(t => t.id !== id));
        }, 3500);
    }, []);

    const dismiss = useCallback((id: number) => {
        setToasts(prev => prev.filter(t => t.id !== id));
    }, []);

    const ToastContainer = () => (
        <div className="fixed bottom-6 right-6 z-[100] flex flex-col gap-2 pointer-events-none">
            {toasts.map(toast => (
                <div
                    key={toast.id}
                    className={`flex items-center gap-3 px-4 py-3 rounded-xl shadow-2xl border text-sm font-medium pointer-events-auto animate-fade-in-up max-w-sm ${toast.type === "success"
                            ? "bg-elevated border-green-500/20 text-green-400"
                            : toast.type === "error"
                                ? "bg-elevated border-red-500/20 text-red-400"
                                : "bg-elevated border-blue-500/20 text-blue-400"
                        }`}
                    style={{ backdropFilter: "blur(12px)" }}
                >
                    <span className="shrink-0">
                        {toast.type === "success" ? (
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M5 13l4 4L19 7" />
                            </svg>
                        ) : toast.type === "error" ? (
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        ) : (
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        )}
                    </span>
                    <span className="flex-1">{toast.message}</span>
                    <button onClick={() => dismiss(toast.id)} className="ml-2 opacity-60 hover:opacity-100 transition-opacity">
                        <svg className="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>
            ))}
        </div>
    );

    return { showToast, ToastContainer };
}
