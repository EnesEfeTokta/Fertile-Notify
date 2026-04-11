import { useNavigate } from "react-router-dom";
import { useEffect, useState, useRef } from "react";

// ─── Helpers ────────────────────────────────────────────────────────────────

const StepCard = ({ n, title, desc }: { n: string; title: string; desc: string }) => (
    <div className="how-step">
        <div className="how-step-number">{n}</div>
        <h3 className="text-base font-bold text-primary mb-1.5">{title}</h3>
        <p className="text-sm text-secondary leading-relaxed">{desc}</p>
    </div>
);

// ─── Animated Channel Orbit ──────────────────────────────────────────────────

const channels = [
    { icon: "✉️", label: "Email",    color: "#3b82f6", angle: 0 },
    { icon: "💬", label: "SMS",      color: "#10b981", angle: 45 },
    { icon: "🔔", label: "Slack",    color: "#f59e0b", angle: 90 },
    { icon: "📲", label: "Push",     color: "#8b5cf6", angle: 135 },
    { icon: "⚡", label: "Webhook",  color: "#ef4444", angle: 180 },
    { icon: "📩", label: "Telegram", color: "#0088cc", angle: 225 },
    { icon: "📱", label: "WhatsApp", color: "#25d366", angle: 270 },
    { icon: "🎮", label: "Discord",  color: "#5865f2", angle: 315 },
];

const ChannelOrbit = () => {
    const [rotation, setRotation] = useState(0);
    const rafRef = useRef<number>(0);

    useEffect(() => {
        let last = 0;
        const tick = (time: number) => {
            if (last) setRotation(prev => prev + (time - last) * 0.018);
            last = time;
            rafRef.current = requestAnimationFrame(tick);
        };
        rafRef.current = requestAnimationFrame(tick);
        return () => cancelAnimationFrame(rafRef.current);
    }, []);

    const R1 = 110; // inner ring radius
    const R2 = 168; // outer ring radius

    return (
        <div className="relative w-[420px] h-[420px] flex items-center justify-center select-none">
            {/* Glow blobs */}
            <div className="absolute inset-0 rounded-full"
                style={{ background: "radial-gradient(circle, rgba(59,130,246,0.13) 0%, transparent 65%)" }} />

            {/* Orbit rings */}
            <div className="absolute inset-0 flex items-center justify-center">
                <div className="absolute rounded-full border border-white/[0.04]" style={{ width: R1 * 2, height: R1 * 2 }} />
                <div className="absolute rounded-full border border-white/[0.04]" style={{ width: R2 * 2, height: R2 * 2 }} />
            </div>

            {/* Center hub */}
            <div className="relative z-10 flex flex-col items-center justify-center w-20 h-20 rounded-full border border-white/10 bg-[#0d0d0d] shadow-xl animate-flow-pulse">
                <div className="w-8 h-8 bg-white rounded-md flex items-center justify-center mb-0.5">
                    <span className="text-black font-bold text-[10px] font-mono">FN</span>
                </div>
                <span className="text-[9px] font-mono text-tertiary tracking-widest">NOTIFY</span>
            </div>

            {/* Orbiting icons */}
            {channels.map((ch, i) => {
                const ring = i % 2 === 0 ? R1 : R2;
                const baseAngle = ch.angle;
                const speed = i % 2 === 0 ? rotation : -rotation * 0.65;
                const rad = ((baseAngle + speed) * Math.PI) / 180;
                const x = Math.cos(rad) * ring;
                const y = Math.sin(rad) * ring;
                return (
                    <div
                        key={ch.label}
                        className="absolute flex flex-col items-center gap-1 transition-transform"
                        style={{ transform: `translate(${x}px, ${y}px)`, willChange: "transform" }}
                    >
                        <div
                            className="w-10 h-10 rounded-xl flex items-center justify-center text-lg shadow-lg"
                            style={{
                                background: `${ch.color}18`,
                                border: `1px solid ${ch.color}40`,
                                boxShadow: `0 4px 20px ${ch.color}25`,
                            }}
                        >
                            {ch.icon}
                        </div>
                        <span className="text-[9px] font-semibold text-tertiary tracking-wide whitespace-nowrap"
                            style={{ textShadow: "0 1px 4px rgba(0,0,0,0.8)" }}>
                            {ch.label}
                        </span>
                    </div>
                );
            })}

            {/* Connection lines (animated dots) */}
            <svg className="absolute inset-0 w-full h-full pointer-events-none" viewBox="0 0 420 420">
                {channels.map((ch, i) => {
                    const ring = i % 2 === 0 ? R1 : R2;
                    const speed = i % 2 === 0 ? rotation : -rotation * 0.65;
                    const rad = ((ch.angle + speed) * Math.PI) / 180;
                    const cx = 210 + Math.cos(rad) * ring;
                    const cy = 210 + Math.sin(rad) * ring;
                    return (
                        <line key={ch.label}
                            x1="210" y1="210"
                            x2={cx} y2={cy}
                            stroke={ch.color}
                            strokeOpacity="0.12"
                            strokeWidth="1"
                            strokeDasharray="4 6"
                        />
                    );
                })}
            </svg>
        </div>
    );
};

// ─── Workflow Diagram ─────────────────────────────────────────────────────────

const workflowSteps = [
    { icon: "⚡", label: "Trigger", color: "#3b82f6", desc: "Event fired" },
    { icon: "🔀", label: "Filter",  color: "#8b5cf6", desc: "Condition check" },
    { icon: "📝", label: "Template", color: "#10b981", desc: "Render content" },
    { icon: "🚀", label: "Dispatch", color: "#f59e0b", desc: "Multi-channel send" },
    { icon: "📊", label: "Track",   color: "#ef4444", desc: "Delivery log" },
];

const WorkflowDiagram = () => {
    const [activeStep, setActiveStep] = useState(0);
    useEffect(() => {
        const id = setInterval(() => setActiveStep(p => (p + 1) % workflowSteps.length), 1200);
        return () => clearInterval(id);
    }, []);

    return (
        <div className="relative w-full">
            {/* Track line */}
            <div className="absolute top-8 left-0 right-0 flex items-center px-[10%] pointer-events-none z-0">
                <div className="h-px flex-1 bg-gradient-to-r from-transparent via-white/10 to-transparent" />
            </div>

            <div className="relative z-10 flex items-start justify-between gap-2">
                {workflowSteps.map((step, i) => {
                    const isActive = i <= activeStep;
                    const isCurrent = i === activeStep;
                    return (
                        <div key={step.label} className="flex flex-col items-center flex-1 gap-3">
                            {/* Node */}
                            <div
                                className="w-16 h-16 rounded-2xl flex items-center justify-center text-2xl transition-all duration-500 relative"
                                style={{
                                    background: isActive ? `${step.color}18` : "var(--bg-card)",
                                    border: `1.5px solid ${isActive ? step.color + "60" : "var(--border-primary)"}`,
                                    boxShadow: isCurrent ? `0 0 0 6px ${step.color}15, 0 8px 32px ${step.color}20` : "none",
                                    transform: isCurrent ? "scale(1.1)" : "scale(1)",
                                }}
                            >
                                {step.icon}
                                {/* Active dot */}
                                {isCurrent && (
                                    <span className="absolute -top-1 -right-1 w-3 h-3 rounded-full border-2 border-black animate-pulse"
                                        style={{ background: step.color }} />
                                )}
                            </div>
                            {/* Label */}
                            <div className="text-center">
                                <p className="text-xs font-bold text-primary">{step.label}</p>
                                <p className="text-[10px] text-tertiary mt-0.5 leading-tight">{step.desc}</p>
                            </div>
                        </div>
                    );
                })}
            </div>

            {/* Animated progress bar */}
            <div className="mt-6 h-px bg-elevated rounded-full overflow-hidden">
                <div
                    className="h-full rounded-full transition-all duration-500"
                    style={{
                        width: `${((activeStep + 1) / workflowSteps.length) * 100}%`,
                        background: "linear-gradient(90deg, #3b82f6, #8b5cf6, #10b981)",
                    }}
                />
            </div>
        </div>
    );
};

// ─── JSON Code Block ──────────────────────────────────────────────────────────

const jsonLines = [
    { indent: 0,   text: "{",                                          type: "brace" },
    { indent: 1,   text: '"eventType":',                               type: "key",    value: ' "ORDER_SHIPPED",' },
    { indent: 1,   text: '"parameters": {',                            type: "key" },
    { indent: 2,   text: '"customer_name":',                           type: "key",    value: ' "Enes",' },
    { indent: 2,   text: '"order_id":',                                type: "key",    value: ' "#FL-2024",' },
    { indent: 2,   text: '"tracking_url":',                            type: "key",    value: ' "https://track.it/FL-2024"' },
    { indent: 1,   text: "},",                                         type: "brace" },
    { indent: 1,   text: '"to": [',                                    type: "key" },
    { indent: 2,   text: "{",                                          type: "brace" },
    { indent: 3,   text: '"channel":',                                 type: "key",    value: ' "email",' },
    { indent: 3,   text: '"recipients":',                              type: "key",    value: ' ["user@example.com"]' },
    { indent: 2,   text: "},",                                         type: "brace" },
    { indent: 2,   text: "{",                                          type: "brace" },
    { indent: 3,   text: '"channel":',                                 type: "key",    value: ' "sms",' },
    { indent: 3,   text: '"recipients":',                              type: "key",    value: ' ["+905550000000"]' },
    { indent: 2,   text: "}",                                          type: "brace" },
    { indent: 1,   text: "]",                                          type: "brace" },
    { indent: 0,   text: "}",                                          type: "brace" },
];

const AnimatedJsonBlock = () => {
    const [visibleLines, setVisibleLines] = useState(0);
    const [done, setDone] = useState(false);

    useEffect(() => {
        if (done) return;
        if (visibleLines >= jsonLines.length) { setDone(true); return; }
        const id = setTimeout(() => setVisibleLines(v => v + 1), 60);
        return () => clearTimeout(id);
    }, [visibleLines, done]);

    // Restart after 5s pause
    useEffect(() => {
        if (!done) return;
        const id = setTimeout(() => { setVisibleLines(0); setDone(false); }, 5000);
        return () => clearTimeout(id);
    }, [done]);

    return (
        <div className="code-block overflow-hidden shadow-accent-lg">
            {/* Window chrome */}
            <div className="flex items-center gap-2 px-4 py-3 border-b border-primary bg-elevated">
                <div className="flex gap-1.5">
                    <div className="w-3 h-3 rounded-full bg-red-500/50" />
                    <div className="w-3 h-3 rounded-full bg-yellow-500/50" />
                    <div className="w-3 h-3 rounded-full bg-green-500/50" />
                </div>
                <span className="text-[11px] text-tertiary font-mono ml-2 flex-1">POST /api/notifications</span>
                <span className="text-[10px] px-2 py-0.5 rounded border bg-blue-500/10 text-blue-400 border-blue-500/20 font-mono font-bold">POST</span>
            </div>

            <div className="p-5 font-mono text-xs leading-6 min-h-[300px]">
                {/* Auth header hint */}
                <div className="text-tertiary mb-4 pb-3 border-b border-white/[0.04]">
                    <span className="text-[#555]">// </span>
                    <span className="text-[#555]">Authorization: Bearer </span>
                    <span className="text-yellow-500/70">YOUR_API_KEY</span>
                </div>

                {jsonLines.slice(0, visibleLines).map((line, i) => (
                    <div key={i} className="flex" style={{ paddingLeft: `${line.indent * 16}px` }}>
                        <span className="text-tertiary w-5 shrink-0 select-none text-right pr-3 text-[10px] leading-6">{i + 1}</span>
                        <span>
                            {line.type === "key" && <span className="text-[#93c5fd]">{line.text}</span>}
                            {line.type === "brace" && <span className="text-[#64748b]">{line.text}</span>}
                            {line.value && (
                                <span className="text-[#86efac]">{line.value}</span>
                            )}
                        </span>
                    </div>
                ))}
                {!done && visibleLines > 0 && (
                    <div className="flex mt-0.5" style={{ paddingLeft: `${(jsonLines[visibleLines - 1]?.indent ?? 0) * 16 + 20}px` }}>
                        <span className="w-1.5 h-4 bg-accent-primary animate-blink rounded-sm" />
                    </div>
                )}
            </div>

            {/* Response status */}
            <div className="flex items-center gap-2 px-4 py-3 border-t border-primary bg-elevated/50">
                <span className="w-2 h-2 rounded-full bg-green-500 animate-pulse" />
                <span className="text-xs text-green-400 font-mono">200 OK · notifications queued · 94ms</span>
            </div>
        </div>
    );
};


// ─── Template Editor Showcase ─────────────────────────────────────────────────

const editorTypes = [
    {
        id: "visual",
        label: "MJML Visual Editor",
        icon: "🎨",
        color: "#8b5cf6",
        desc: "Drag-and-drop blocks, live preview, responsive-by-default.",
        preview: (
            <div className="text-[10px] font-mono space-y-1.5 text-left p-3">
                <div className="flex gap-2 items-center">
                    <div className="w-16 h-5 rounded bg-blue-500/20 border border-blue-500/20 flex items-center px-1.5">
                        <span className="text-[9px] text-blue-400">Header</span>
                    </div>
                    <div className="w-5 h-5 rounded bg-white/5 border border-white/10 text-center text-[9px] text-tertiary leading-5">⋮</div>
                </div>
                <div className="h-8 rounded bg-purple-500/10 border border-purple-500/20 flex items-center px-2">
                    <span className="text-[9px] text-purple-400">Hero Image Block</span>
                </div>
                <div className="grid grid-cols-2 gap-1.5">
                    <div className="h-6 rounded bg-white/5 border border-white/10 flex items-center px-1.5">
                        <span className="text-[9px] text-tertiary">Text column</span>
                    </div>
                    <div className="h-6 rounded bg-white/5 border border-white/10 flex items-center px-1.5">
                        <span className="text-[9px] text-tertiary">Button column</span>
                    </div>
                </div>
                <div className="h-5 rounded bg-green-500/10 border border-green-500/20 flex items-center px-2">
                    <span className="text-[9px] text-green-400">CTA Button</span>
                </div>
            </div>
        )
    },
    {
        id: "code",
        label: "Advanced Code Editor",
        icon: "💻",
        color: "#3b82f6",
        desc: "Raw MJML / HTML with Monaco-powered syntax highlighting and live compile.",
        preview: (
            <div className="text-[10px] font-mono p-3 space-y-0.5 text-left">
                <div><span className="text-purple-400">&lt;mj-section&gt;</span></div>
                <div className="pl-4"><span className="text-blue-400">&lt;mj-column&gt;</span></div>
                <div className="pl-8"><span className="text-blue-400">&lt;mj-text </span><span className="text-yellow-400">font-size</span><span className="text-blue-400">=</span><span className="text-green-400">"18px"</span><span className="text-blue-400">&gt;</span></div>
                <div className="pl-12 text-tertiary">{"{{customer_name}}"}</div>
                <div className="pl-8"><span className="text-blue-400">&lt;/mj-text&gt;</span></div>
                <div className="pl-4"><span className="text-blue-400">&lt;/mj-column&gt;</span></div>
                <div><span className="text-purple-400">&lt;/mj-section&gt;</span></div>
            </div>
        )
    },
    {
        id: "channel",
        label: "Channel Templates",
        icon: "📡",
        color: "#10b981",
        desc: "Channel-specific templates for SMS, Slack blocks, Telegram markdown, and more.",
        preview: (
            <div className="text-[10px] font-mono p-3 space-y-1.5 text-left">
                <div className="flex items-center gap-1.5">
                    <span className="text-green-400 font-bold">SMS</span>
                    <div className="flex-1 h-px bg-white/[0.06]" />
                </div>
                <div className="bg-white/[0.03] rounded p-1.5 text-tertiary leading-4">
                    Hi {"{{name}}"}, your order {"{{id}}"} shipped! Track: {"{{url}}"}
                </div>
                <div className="flex items-center gap-1.5 mt-2">
                    <span className="text-yellow-400 font-bold">Slack</span>
                    <div className="flex-1 h-px bg-white/[0.06]" />
                </div>
                <div className="bg-white/[0.03] rounded p-1.5 text-tertiary leading-4">
                    :package: *Order shipped!* <br />
                    <span className="text-blue-400">{"{{tracking_url}}"}</span>
                </div>
            </div>
        )
    },
];

const TemplateEditorShowcase = () => {
    const [active, setActive] = useState(0);
    return (
        <div className="w-full">
            {/* Tab bar */}
            <div className="flex gap-2 mb-5 flex-wrap">
                {editorTypes.map((e, i) => (
                    <button
                        key={e.id}
                        onClick={() => setActive(i)}
                        className={`flex items-center gap-2 px-4 py-2 rounded-lg text-xs font-semibold transition-all duration-200 border ${
                            active === i
                                ? "text-primary border-transparent"
                                : "bg-transparent text-tertiary border-primary hover:text-secondary hover:border-hover"
                        }`}
                        style={active === i ? {
                            background: `${e.color}15`,
                            borderColor: `${e.color}40`,
                            color: e.color,
                        } : {}}
                    >
                        <span>{e.icon}</span>
                        {e.label}
                    </button>
                ))}
            </div>

            {/* Editor preview window */}
            <div className="code-block overflow-hidden shadow-accent-lg">
                <div className="flex items-center gap-2 px-4 py-3 border-b border-primary bg-elevated">
                    <div className="flex gap-1.5">
                        <div className="w-3 h-3 rounded-full bg-red-500/50" />
                        <div className="w-3 h-3 rounded-full bg-yellow-500/50" />
                        <div className="w-3 h-3 rounded-full bg-green-500/50" />
                    </div>
                    <span className="text-[11px] text-tertiary font-mono ml-2">{editorTypes[active].label}</span>
                    <div className="ml-auto flex items-center gap-1.5">
                        <span className="w-1.5 h-1.5 rounded-full bg-green-500 animate-pulse" />
                        <span className="text-[10px] text-green-400 font-mono">Live preview</span>
                    </div>
                </div>
                <div className="grid grid-cols-2 min-h-[180px]">
                    {/* Left: editor mock */}
                    <div className="border-r border-white/[0.04]">
                        {editorTypes[active].preview}
                    </div>
                    {/* Right: live preview */}
                    <div className="p-3 flex flex-col items-center justify-center gap-2">
                        <div className="w-24 h-3 rounded-full bg-white/10" />
                        <div className="w-16 h-2 rounded-full bg-white/5" />
                        <div className="w-20 h-6 rounded-lg mt-2"
                            style={{ background: `${editorTypes[active].color}25`, border: `1px solid ${editorTypes[active].color}40` }}>
                            <div className="w-full h-full flex items-center justify-center">
                                <span className="text-[9px] font-semibold" style={{ color: editorTypes[active].color }}>Send Now →</span>
                            </div>
                        </div>
                        <span className="text-[9px] text-tertiary mt-1 font-mono">preview.html</span>
                    </div>
                </div>
            </div>
        </div>
    );
};

// ─── Feature Card ─────────────────────────────────────────────────────────────

const FeatureCard = ({
    icon, title, description, tags
}: {
    icon: string; title: string; description: string; tags: string[];
}) => (
    <div className="card p-7 group hover-reveal-border transition-smooth relative overflow-hidden h-full flex flex-col">
        <div className="text-3xl mb-5">{icon}</div>
        <h3 className="text-base font-bold text-primary mb-2 group-hover:text-accent-light transition-colors">{title}</h3>
        <p className="text-sm text-secondary leading-relaxed mb-4 flex-1">{description}</p>
        <div className="flex flex-wrap gap-1.5 mt-auto">
            {tags.map(tag => (
                <span key={tag} className="tag">{tag}</span>
            ))}
        </div>
    </div>
);

// ─── Dispatch Hub (Hero Visual) ──────────────────────────────────────────────

const hubChannels = [
    { icon: "✉️", label: "Email",    color: "#3b82f6", angle: -75, desc: "Welcome email sent! 📬" },
    { icon: "💬", label: "SMS",      color: "#10b981", angle: -15, desc: "OTP: 847291 🔐" },
    { icon: "🔔", label: "Slack",    color: "#f59e0b", angle: 45,  desc: "#dev: Deploy done ✅" },
    { icon: "📲", label: "Push",     color: "#8b5cf6", angle: 105, desc: "New order arrived 📦" },
    { icon: "⚡", label: "Webhook",  color: "#ef4444", angle: 165, desc: "Payload delivered ⚡" },
    { icon: "📩", label: "Telegram", color: "#0088cc", angle: -135, desc: "Payment received 💳" },
];

const DispatchHub = () => {
    const [activeIdx, setActiveIdx] = useState(0);
    const [signalProgress, setSignalProgress] = useState(0);
    const [totalDispatched, setTotalDispatched] = useState(24839);
    const rafRef = useRef<number>(0);

    useEffect(() => {
        const id = setInterval(() => {
            setActiveIdx(prev => (prev + 1) % hubChannels.length);
            setTotalDispatched(prev => prev + Math.floor(Math.random() * 4) + 1);
        }, 2200);
        return () => clearInterval(id);
    }, []);

    useEffect(() => {
        setSignalProgress(0);
        let startTime: number | null = null;
        const DURATION = 750;
        const animate = (time: number) => {
            if (!startTime) startTime = time;
            const p = Math.min((time - startTime) / DURATION, 1);
            setSignalProgress(1 - Math.pow(1 - p, 3)); // ease-out cubic
            if (p < 1) rafRef.current = requestAnimationFrame(animate);
        };
        rafRef.current = requestAnimationFrame(animate);
        return () => cancelAnimationFrame(rafRef.current);
    }, [activeIdx]);

    const SIZE = 460;
    const CX = SIZE / 2;
    const CY = SIZE / 2;
    const R = 158;
    const activeCh = hubChannels[activeIdx];

    return (
        <div className="relative select-none" style={{ width: SIZE, height: SIZE }}>
            {/* Dynamic ambient glow */}
            <div className="absolute inset-0 rounded-full pointer-events-none"
                style={{
                    background: `radial-gradient(circle at center, ${activeCh.color}12 0%, transparent 65%)`,
                    transition: "background 1s ease",
                }} />

            {/* SVG: rings + lines + animated signal dot */}
            <svg className="absolute inset-0" width={SIZE} height={SIZE} viewBox={`0 0 ${SIZE} ${SIZE}`}>
                <defs>
                    <filter id="hub-glow" x="-50%" y="-50%" width="200%" height="200%">
                        <feGaussianBlur stdDeviation="3" result="blur" />
                        <feMerge><feMergeNode in="blur" /><feMergeNode in="SourceGraphic" /></feMerge>
                    </filter>
                </defs>
                <circle cx={CX} cy={CY} r={R} fill="none" stroke="rgba(255,255,255,0.04)" strokeWidth="1" strokeDasharray="6 12" />
                <circle cx={CX} cy={CY} r={52} fill="none" stroke="rgba(255,255,255,0.05)" strokeWidth="1" />
                {hubChannels.map((ch, i) => {
                    const rad = (ch.angle * Math.PI) / 180;
                    const x2 = CX + Math.cos(rad) * R;
                    const y2 = CY + Math.sin(rad) * R;
                    const isActive = i === activeIdx;
                    return (
                        <line key={ch.label}
                            x1={CX} y1={CY} x2={x2} y2={y2}
                            stroke={ch.color}
                            strokeOpacity={isActive ? 0.35 : 0.07}
                            strokeWidth={isActive ? 1.5 : 1}
                            strokeDasharray={isActive ? "none" : "5 10"}
                            style={{ transition: "stroke-opacity 0.6s ease, stroke-width 0.4s ease" }}
                        />
                    );
                })}
                {(() => {
                    const rad = (activeCh.angle * Math.PI) / 180;
                    const tx = CX + Math.cos(rad) * R;
                    const ty = CY + Math.sin(rad) * R;
                    const x = CX + (tx - CX) * signalProgress;
                    const y = CY + (ty - CY) * signalProgress;
                    const fade = signalProgress < 0.15
                        ? signalProgress / 0.15
                        : signalProgress > 0.85 ? (1 - signalProgress) / 0.15 : 1;
                    return (
                        <g filter="url(#hub-glow)" opacity={fade}>
                            <circle cx={x} cy={y} r="5" fill={activeCh.color} />
                            <circle cx={x} cy={y} r="11" fill={activeCh.color} fillOpacity="0.2" />
                        </g>
                    );
                })()}
            </svg>

            {/* Center hub */}
            <div className="absolute z-10 flex flex-col items-center justify-center rounded-full"
                style={{
                    width: 100, height: 100,
                    left: CX - 50, top: CY - 50,
                    background: "rgba(10,10,10,0.97)",
                    border: `2px solid ${activeCh.color}55`,
                    boxShadow: `0 0 30px ${activeCh.color}20, 0 0 80px ${activeCh.color}08, inset 0 1px 0 rgba(255,255,255,0.05)`,
                    transition: "border-color 0.7s ease, box-shadow 0.7s ease",
                }}>
                <div className="absolute inset-[-7px] rounded-full animate-flow-pulse"
                    style={{ border: `1px solid ${activeCh.color}20`, transition: "border-color 0.7s ease" }} />
                <div className="w-9 h-9 bg-white rounded-lg flex items-center justify-center mb-1">
                    <span className="text-black font-bold text-[11px] font-mono">FN</span>
                </div>
                <span className="text-[9px] font-mono text-tertiary tracking-widest uppercase">Notify</span>
            </div>

            {/* Channel nodes */}
            {hubChannels.map((ch, i) => {
                const rad = (ch.angle * Math.PI) / 180;
                const x = CX + Math.cos(rad) * R;
                const y = CY + Math.sin(rad) * R;
                const isActive = i === activeIdx;
                const cosA = Math.cos(rad);
                const sinA = Math.sin(rad);
                const popupStyle: React.CSSProperties = {};
                if (Math.abs(cosA) > Math.abs(sinA)) {
                    if (cosA > 0) { popupStyle.left = "calc(100% + 10px)"; popupStyle.top = "50%"; popupStyle.transform = "translateY(-50%)"; }
                    else          { popupStyle.right = "calc(100% + 10px)"; popupStyle.top = "50%"; popupStyle.transform = "translateY(-50%)"; }
                } else {
                    if (sinA < 0) { popupStyle.bottom = "calc(100% + 10px)"; popupStyle.left = "50%"; popupStyle.transform = "translateX(-50%)"; }
                    else          { popupStyle.top = "calc(100% + 10px)"; popupStyle.left = "50%"; popupStyle.transform = "translateX(-50%)"; }
                }
                return (
                    <div key={ch.label}
                        className="absolute flex flex-col items-center gap-1.5 z-10"
                        style={{ left: x, top: y, transform: "translate(-50%,-50%)" }}>
                        <div className="w-[54px] h-[54px] rounded-2xl flex items-center justify-center text-xl"
                            style={{
                                background: isActive ? `${ch.color}22` : "rgba(14,14,14,0.95)",
                                border: `1.5px solid ${isActive ? ch.color + "65" : "rgba(255,255,255,0.07)"}`,
                                boxShadow: isActive
                                    ? `0 0 28px ${ch.color}30, 0 8px 32px rgba(0,0,0,0.6)`
                                    : "0 4px 20px rgba(0,0,0,0.4)",
                                transform: isActive ? "scale(1.18)" : "scale(1)",
                                transition: "all 0.5s cubic-bezier(0.34,1.56,0.64,1)",
                            }}>
                            {ch.icon}
                        </div>
                        <span className="text-[10px] font-bold tracking-wide"
                            style={{ color: isActive ? ch.color : "var(--text-muted)", transition: "color 0.4s ease" }}>
                            {ch.label}
                        </span>
                        {isActive && signalProgress > 0.88 && (
                            <div className="absolute animate-scale-in z-20 pointer-events-none"
                                style={{
                                    ...popupStyle,
                                    background: "rgba(11,11,11,0.97)",
                                    backdropFilter: "blur(20px)",
                                    border: `1px solid ${ch.color}28`,
                                    borderRadius: 10,
                                    padding: "8px 13px",
                                    whiteSpace: "nowrap",
                                    boxShadow: `0 8px 40px rgba(0,0,0,0.7)`,
                                }}>
                                <div className="flex items-center gap-2">
                                    <span className="w-1.5 h-1.5 rounded-full animate-pulse shrink-0" style={{ background: ch.color }} />
                                    <p className="text-[11px] text-white font-medium">{ch.desc}</p>
                                </div>
                            </div>
                        )}
                    </div>
                );
            })}

            {/* Live counter badge */}
            <div className="absolute bottom-3 left-1/2 -translate-x-1/2 z-10 flex items-center gap-3 px-5 py-2.5"
                style={{
                    background: "rgba(10,10,10,0.92)",
                    backdropFilter: "blur(20px)",
                    border: "1px solid rgba(255,255,255,0.07)",
                    borderRadius: 12,
                    boxShadow: "0 8px 32px rgba(0,0,0,0.5)",
                }}>
                <span className="w-1.5 h-1.5 rounded-full bg-green-400 animate-pulse shrink-0" />
                <div>
                    <p className="text-[9px] uppercase tracking-widest text-tertiary font-mono">dispatched today</p>
                    <p className="font-display font-bold text-base text-primary leading-tight tabular-nums">
                        {totalDispatched.toLocaleString()}
                    </p>
                </div>
            </div>
        </div>
    );
};

// ─── Page ─────────────────────────────────────────────────────────────────────

export default function HomePage() {
    const navigate = useNavigate();
    const [scrolled, setScrolled] = useState(false);
    const isAuthenticated = !!localStorage.getItem("accessToken");

    useEffect(() => {
        const handleScroll = () => setScrolled(window.scrollY > 40);
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    // Cycling word in hero headline
    const heroWords  = ["Email", "SMS", "Slack", "Push", "Webhook"];
    const heroColors = ["#3b82f6", "#10b981", "#f59e0b", "#8b5cf6", "#ef4444"];
    const [heroWordIdx, setHeroWordIdx] = useState(0);
    const [heroFade, setHeroFade] = useState(true);

    useEffect(() => {
        const id = setInterval(() => {
            setHeroFade(false);
            setTimeout(() => {
                setHeroWordIdx(prev => (prev + 1) % heroWords.length);
                setHeroFade(true);
            }, 280);
        }, 2000);
        return () => clearInterval(id);
    }, []);


    const features = [
        {
            icon: "✉️", title: "Email Service", tags: ["MJML", "HTML", "Templates"],
            description: "Deliver pixel-perfect, responsive emails using our visual MJML editor or raw HTML API. Supports variables, layouts, and live preview."
        },
        {
            icon: "💬", title: "SMS & Messaging", tags: ["SMS", "Two-way", "Global"],
            description: "Reach your users globally with lightning-fast SMS delivery. Mobile OTPs, marketing blasts, and transactional messages in one place."
        },
        {
            icon: "🔔", title: "Slack & Discord", tags: ["Slack", "Discord", "Webhooks"],
            description: "Native integrations for team collaboration tools. Send rich blocks to Slack or messages to Discord channels without writing any boilerplate."
        },
        {
            icon: "📲", title: "Push Notifications", tags: ["FCM", "Web", "Mobile"],
            description: "Keep engagement high with Web Push and Mobile push powered by FCM. Supports background notifications and custom payloads."
        },
        {
            icon: "📩", title: "Telegram & WhatsApp", tags: ["Telegram Bot", "WhatsApp API"],
            description: "Connect with your audience on the platforms they already use—no onboarding friction. Works with Telegram Bots and WhatsApp Business API."
        },
        {
            icon: "⚡", title: "Webhooks", tags: ["Any URL", "Retries", "Secure"],
            description: "Maximum flexibility. Forward event data to any HTTP endpoint in your stack with automatic retries and signature verification."
        }
    ];

    return (
        <div className="min-h-screen bg-primary text-primary overflow-x-hidden">
            {/* Background */}
            <div className="fixed inset-0 bg-grid-fine pointer-events-none z-0" />
            <div className="fixed inset-0 glow-overlay z-0" />

            {/* ── NAV ── */}
            <nav
                className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 ${scrolled
                    ? "bg-primary/85 backdrop-blur-md border-b border-primary py-3.5"
                    : "bg-transparent py-5"
                }`}
            >
                <div className="max-w-7xl mx-auto px-6 flex justify-between items-center">
                    <div className="flex items-center gap-10">
                        <a
                            href="/"
                            className="flex items-center gap-2.5 group"
                            onClick={e => { e.preventDefault(); window.scrollTo({ top: 0, behavior: "smooth" }); }}
                        >
                            <div className="w-7 h-7 bg-white rounded-md flex items-center justify-center">
                                <span className="text-black font-bold text-xs font-mono">FN</span>
                            </div>
                            <span className="font-display font-bold text-[15px] tracking-tight">
                                fertile<span className="text-accent-primary">notify</span>
                            </span>
                        </a>
                        <div className="hidden md:flex items-center gap-1">
                            {[
                                { label: "Features",  action: () => document.getElementById("features")?.scrollIntoView({ behavior: "smooth" }) },
                                { label: "Pricing",   action: () => navigate("/pricing") },
                                { label: "Changelog", action: () => navigate("/changelog") },
                            ].map(l => (
                                <button key={l.label} onClick={l.action} className="btn-ghost text-[13px] py-1.5">
                                    {l.label}
                                </button>
                            ))}
                            {isAuthenticated && (
                                <>
                                    <button onClick={() => navigate("/templates")}  className="btn-ghost text-[13px] py-1.5">Templates</button>
                                    <button onClick={() => navigate("/statistics")} className="btn-ghost text-[13px] py-1.5">Statistics</button>
                                </>
                            )}
                        </div>
                    </div>
                    <div className="flex items-center gap-2.5">
                        {!isAuthenticated ? (
                            <>
                                <button onClick={() => navigate("/login")}    className="btn-ghost text-[13px]">Sign in</button>
                                <button onClick={() => navigate("/register")} className="btn-primary text-[13px]">
                                    Get Started
                                    <svg className="ml-1.5 w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                                    </svg>
                                </button>
                            </>
                        ) : (
                            <button onClick={() => navigate("/dashboard")} className="btn-primary text-[13px]">
                                Dashboard
                                <svg className="ml-1.5 w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                                </svg>
                            </button>
                        )}
                    </div>
                </div>
            </nav>

            {/* ══════════════════════════════════════════════════════════════════════ */}
            {/* HERO                                                                  */}
            {/* ══════════════════════════════════════════════════════════════════════ */}
            <section className="relative min-h-screen flex items-center pt-20 pb-24 px-6 overflow-hidden">
                {/* Cinematic background */}
                <div className="absolute inset-0 pointer-events-none"
                    style={{ background: "radial-gradient(ellipse 80% 70% at 65% 50%, rgba(59,130,246,0.07) 0%, transparent 65%)" }} />

                <div className="max-w-7xl mx-auto w-full grid grid-cols-1 lg:grid-cols-[1fr_auto] items-center gap-12 xl:gap-20">
                    {/* Left */}
                    <div className="relative z-10 max-w-xl">
                        {/* Badge */}
                        <div className="section-label animate-fade-in">
                            <span className="w-1.5 h-1.5 rounded-full bg-green-400 animate-pulse" />
                            <span>Open Beta — Free to start</span>
                        </div>

                        {/* Headline with cycling channel word */}
                        <h1 className="hero-text-xl mt-5 mb-5 animate-fade-in-up">
                            Send to<br />
                            <span
                                className="inline-block"
                                style={{
                                    color: heroColors[heroWordIdx],
                                    opacity: heroFade ? 1 : 0,
                                    transform: heroFade ? "translateY(0)" : "translateY(8px)",
                                    transition: "opacity 0.28s ease, transform 0.28s ease, color 0s",
                                    textShadow: `0 0 80px ${heroColors[heroWordIdx]}35`,
                                }}
                            >
                                {heroWords[heroWordIdx]}.
                            </span>
                        </h1>

                        {/* API endpoint badge */}
                        <div className="inline-flex items-center gap-2.5 mb-6 px-4 py-2 rounded-xl animate-fade-in-up delay-100"
                            style={{
                                background: "rgba(59,130,246,0.07)",
                                border: "1px solid rgba(59,130,246,0.18)",
                            }}>
                            <span className="text-[10px] font-bold text-blue-400 px-1.5 py-0.5 rounded font-mono"
                                style={{ background: "rgba(59,130,246,0.15)", border: "1px solid rgba(59,130,246,0.25)" }}>
                                POST
                            </span>
                            <span className="font-mono text-[12px] text-tertiary">
                                /api/<span className="text-blue-300">notifications</span>
                            </span>
                            <span className="text-tertiary text-[11px]">→ any channel</span>
                        </div>

                        <p className="text-[17px] text-secondary leading-relaxed mb-8 animate-fade-in-up delay-200">
                            Fertile Notify routes a single API call to Email, SMS, Slack, Push, Webhooks, and more.
                            One unified infrastructure — no SDK juggling.
                        </p>

                        {/* CTA */}
                        <div className="flex flex-col sm:flex-row items-start gap-3 animate-fade-in-up delay-300">
                            {!isAuthenticated ? (
                                <button onClick={() => navigate("/register")} className="btn-primary px-7 py-3 text-[15px] shadow-accent">
                                    Start for free
                                    <svg className="ml-2 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                                    </svg>
                                </button>
                            ) : (
                                <button onClick={() => navigate("/dashboard")} className="btn-primary px-7 py-3 text-[15px] shadow-accent">
                                    Go to Dashboard
                                    <svg className="ml-2 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7l5 5m0 0l-5 5m5-5H6" />
                                    </svg>
                                </button>
                            )}
                            <button onClick={() => navigate("/api-reference")} className="btn-secondary px-7 py-3 text-[15px]">
                                <span className="font-mono text-accent-light mr-2">{"<"}{">"}</span>
                                View API Docs
                            </button>
                        </div>

                        {/* Stats row */}
                        <div className="grid grid-cols-3 gap-6 mt-10 pt-8 border-t border-primary animate-fade-in-up delay-400">
                            {[
                                { value: "6+",    label: "Channels" },
                                { value: "2,000", label: "Free / month" },
                                { value: "99.9%", label: "Uptime SLA" },
                            ].map(s => (
                                <div key={s.label}>
                                    <p className="font-display font-bold text-2xl text-primary tracking-tight leading-none">{s.value}</p>
                                    <p className="text-[11px] text-tertiary mt-1.5">{s.label}</p>
                                </div>
                            ))}
                        </div>
                    </div>

                    {/* Right: Dispatch Hub */}
                    <div className="hidden lg:flex items-center justify-center">
                        <DispatchHub />
                    </div>
                </div>

                {/* Bottom fade */}
                <div className="absolute bottom-0 left-0 right-0 h-36 pointer-events-none"
                    style={{ background: "linear-gradient(to top, var(--bg-primary), transparent)" }} />
            </section>


            {/* ══════════════════════════════════════════════════════════════════════ */}
            {/* CHANNEL ORBIT — Apple-style animated channels                         */}
            {/* ══════════════════════════════════════════════════════════════════════ */}
            <section className="py-28 px-6 relative overflow-hidden border-y border-primary bg-secondary/10">
                {/* Ambient glow */}
                <div className="absolute inset-0 pointer-events-none"
                    style={{ background: "radial-gradient(ellipse 70% 60% at 50% 50%, rgba(59,130,246,0.06) 0%, transparent 70%)" }} />

                <div className="max-w-7xl mx-auto flex flex-col lg:flex-row items-center gap-20">
                    {/* Left: orbit */}
                    <div className="flex-1 flex justify-center items-center">
                        <ChannelOrbit />
                    </div>

                    {/* Right: text */}
                    <div className="lg:w-[45%] space-y-6">
                        <div className="section-label">Multi-Channel</div>
                        <h2 className="font-display font-bold text-4xl md:text-5xl leading-tight tracking-tight">
                            Every channel,<br />
                            <span className="gradient-text-blue">one platform.</span>
                        </h2>
                        <p className="text-secondary text-lg leading-relaxed">
                            From Email to SMS, Slack to Telegram, Push to Webhook — Fertile Notify connects to
                            every channel your users live on. No separate SDKs. No duct tape. Just one API.
                        </p>
                        <div className="grid grid-cols-2 gap-3 pt-2">
                            {channels.map(ch => (
                                <div key={ch.label} className="flex items-center gap-3 card p-3">
                                    <div className="w-8 h-8 rounded-lg flex items-center justify-center text-base shrink-0"
                                        style={{ background: `${ch.color}18`, border: `1px solid ${ch.color}35` }}>
                                        {ch.icon}
                                    </div>
                                    <span className="text-sm font-medium text-secondary">{ch.label}</span>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </section>

            {/* ══════════════════════════════════════════════════════════════════════ */}
            {/* HOW IT WORKS                                                          */}
            {/* ══════════════════════════════════════════════════════════════════════ */}
            <section className="py-28 px-6 relative">
                <div className="max-w-6xl mx-auto">
                    <div className="text-center mb-16">
                        <div className="section-label mx-auto">How it works</div>
                        <h2 className="hero-text text-4xl md:text-5xl mt-2">
                            Integrate in minutes,<br />
                            <span className="text-secondary font-light">not weeks.</span>
                        </h2>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
                        <StepCard n="01" title="Connect your channels" desc="Add your SMTP, Twilio, Slack webhook, or FCM credentials in the dashboard settings." />
                        <StepCard n="02" title="Design a template"     desc="Use our visual MJML editor or write plain text. Set variables with Handlebars syntax." />
                        <StepCard n="03" title="Call our single API"   desc="POST to /api/notifications with your API key, template ID, and dynamic data. That's it." />
                        <StepCard n="04" title="Track delivery"        desc="Monitor every delivery, open, and failure from our unified statistics dashboard." />
                    </div>
                </div>
            </section>

            {/* ══════════════════════════════════════════════════════════════════════ */}
            {/* WORKFLOW SECTION                                                       */}
            {/* ══════════════════════════════════════════════════════════════════════ */}
            <section className="py-28 px-6 relative overflow-hidden bg-secondary/10 border-y border-primary">
                {/* Decorative blobs */}
                <div className="absolute top-0 left-1/4 w-96 h-96 bg-purple-500/5 blur-[120px] rounded-full pointer-events-none" />
                <div className="absolute bottom-0 right-1/4 w-96 h-96 bg-blue-500/5 blur-[120px] rounded-full pointer-events-none" />

                <div className="max-w-6xl mx-auto">
                    <div className="text-center mb-16">
                        <div className="section-label mx-auto">Workflows</div>
                        <h2 className="hero-text text-4xl md:text-5xl mt-2">
                            Automate notification<br />
                            <span className="gradient-text-blue">pipelines visually.</span>
                        </h2>
                        <p className="text-secondary text-lg max-w-2xl mx-auto mt-5 leading-relaxed">
                            Design multi-step notification flows with triggers, filters, template rendering, and
                            multi-channel dispatching — all in one draggable canvas.
                        </p>
                    </div>

                    {/* Workflow diagram */}
                    <div className="card p-8 mb-10">
                        <WorkflowDiagram />
                    </div>

                    {/* Workflow feature grid */}
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-5">
                        {[
                            {
                                icon: "⚡", color: "#3b82f6",
                                title: "Event Triggers",
                                desc: "Kick off workflows from API events, schedules, or inbound webhooks from any system."
                            },
                            {
                                icon: "🔀", color: "#8b5cf6",
                                title: "Conditional Branching",
                                desc: "Route notifications based on recipient attributes, payload data, or previous step outcomes."
                            },
                            {
                                icon: "🚀", color: "#10b981",
                                title: "Parallel Dispatch",
                                desc: "Send to Email, SMS, Slack, and Push simultaneously in a single workflow execution."
                            },
                        ].map(f => (
                            <div key={f.title} className="card p-6 group hover-reveal-border transition-smooth">
                                <div className="w-10 h-10 rounded-xl flex items-center justify-center text-xl mb-4 transition-transform group-hover:scale-110"
                                    style={{ background: `${f.color}15`, border: `1px solid ${f.color}35` }}>
                                    {f.icon}
                                </div>
                                <h3 className="text-sm font-bold text-primary mb-2">{f.title}</h3>
                                <p className="text-sm text-secondary leading-relaxed">{f.desc}</p>
                            </div>
                        ))}
                    </div>

                    <div className="text-center mt-10">
                        <button onClick={() => navigate("/workflows")} className="btn-secondary px-7 py-3">
                            Open Workflow Builder →
                        </button>
                    </div>
                </div>
            </section>

            {/* ══════════════════════════════════════════════════════════════════════ */}
            {/* FEATURES (Channels grid)                                              */}
            {/* ══════════════════════════════════════════════════════════════════════ */}
            <section id="features" className="py-28 px-6">
                <div className="max-w-7xl mx-auto">
                    <div className="text-center mb-16">
                        <div className="section-label mx-auto">Channels</div>
                        <h2 className="hero-text text-4xl md:text-5xl mt-2">
                            Omnichannel is<br />
                            <span className="gradient-text-blue">the default.</span>
                        </h2>
                        <p className="text-secondary text-lg max-w-xl mx-auto mt-4">
                            Stop rebuilding integrations from scratch. Our engine handles all the complexity.
                        </p>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
                        {features.map((f, i) => (
                            <FeatureCard key={i} {...f} />
                        ))}
                    </div>
                </div>
            </section>

            {/* ══════════════════════════════════════════════════════════════════════ */}
            {/* TEMPLATE EDITOR SHOWCASE                                              */}
            {/* ══════════════════════════════════════════════════════════════════════ */}
            <section className="py-28 px-6 relative overflow-hidden border-y border-primary bg-secondary/10">
                <div className="absolute top-0 right-0 w-[500px] h-[500px] pointer-events-none"
                    style={{ background: "radial-gradient(ellipse at top right, rgba(139,92,246,0.07) 0%, transparent 70%)" }} />

                <div className="max-w-7xl mx-auto flex flex-col lg:flex-row items-center gap-20">
                    {/* Left: text */}
                    <div className="lg:w-[40%] space-y-6">
                        <div className="section-label">Template Designers</div>
                        <h2 className="font-display font-bold text-4xl md:text-5xl leading-tight tracking-tight">
                            Design beautiful<br />
                            <span className="gradient-text-blue">notifications.</span>
                        </h2>
                        <p className="text-secondary text-lg leading-relaxed">
                            Choose your workflow: drag-and-drop MJML blocks, raw HTML with Monaco intellisense,
                            or channel-specific templates. All editors support Handlebars variable interpolation
                            and live preview out of the box.
                        </p>
                        <ul className="space-y-3.5">
                            {[
                                "Visual MJML drag-and-drop builder",
                                "Monaco-powered advanced code editor",
                                "Handlebars {{variable}} interpolation",
                                "Live HTML/MJML preview pane",
                                "Per-channel template management",
                            ].map(item => (
                                <li key={item} className="flex items-center gap-3 text-sm">
                                    <div className="w-5 h-5 rounded-full bg-accent-dim border border-blue-500/20 flex items-center justify-center text-accent-primary text-[10px]">✓</div>
                                    <span className="text-secondary">{item}</span>
                                </li>
                            ))}
                        </ul>
                        <button onClick={() => navigate("/templates")} className="btn-secondary mt-2">
                            Open Template Editor →
                        </button>
                    </div>

                    {/* Right: editor showcase */}
                    <div className="lg:w-[60%] w-full">
                        <TemplateEditorShowcase />
                    </div>
                </div>
            </section>

            {/* ══════════════════════════════════════════════════════════════════════ */}
            {/* API — JSON showcase                                                   */}
            {/* ══════════════════════════════════════════════════════════════════════ */}
            <section className="py-28 px-6 relative overflow-hidden">
                <div className="absolute inset-0 pointer-events-none"
                    style={{ background: "radial-gradient(ellipse 60% 50% at 30% 60%, rgba(59,130,246,0.05) 0%, transparent 70%)" }} />

                <div className="max-w-7xl mx-auto flex flex-col lg:flex-row items-center gap-20">
                    {/* Left: text */}
                    <div className="lg:w-1/2 space-y-6">
                        <div className="section-label">Developer First</div>
                        <h2 className="font-display font-bold text-4xl md:text-5xl leading-tight tracking-tight">
                            One endpoint.<br />
                            <span className="text-secondary font-light">All channels.</span>
                        </h2>
                        <p className="text-secondary text-lg leading-relaxed">
                            Our REST API is designed for clarity. Authenticate once, then send to Email, SMS, Slack,
                            and more with a single JSON body. Rate limiting, automatic retries, and delivery
                            webhooks are included by default.
                        </p>

                        {/* Endpoint snippet */}
                        <div className="card p-4 flex items-center gap-3 border-l-2 border-accent-primary">
                            <span className="text-[10px] font-bold px-2.5 py-1 rounded border font-mono bg-blue-500/10 text-blue-400 border-blue-500/20 shrink-0">POST</span>
                            <code className="font-mono text-sm text-primary">/api/notifications</code>
                        </div>

                        <ul className="space-y-3.5">
                            {[
                                "MJML-based responsive email engine",
                                "Handlebars variable interpolation",
                                "Automatic retries on failure",
                                "Signed webhook verification",
                                "Delivery status webhooks",
                            ].map(item => (
                                <li key={item} className="flex items-center gap-3 text-sm">
                                    <div className="w-5 h-5 rounded-full bg-accent-dim border border-blue-500/20 flex items-center justify-center text-accent-primary text-[10px]">✓</div>
                                    <span className="text-secondary">{item}</span>
                                </li>
                            ))}
                        </ul>

                        <button onClick={() => navigate("/api-reference")} className="btn-secondary mt-2">
                            View full API reference →
                        </button>
                    </div>

                    {/* Right: animated JSON block */}
                    <div className="lg:w-1/2 w-full">
                        <AnimatedJsonBlock />
                    </div>
                </div>
            </section>

            {/* ══════════════════════════════════════════════════════════════════════ */}
            {/* PRICING TEASER                                                        */}
            {/* ══════════════════════════════════════════════════════════════════════ */}
            <section className="py-28 px-6 border-y border-primary bg-secondary/10">
                <div className="max-w-5xl mx-auto text-center">
                    <div className="section-label mx-auto">Pricing</div>
                    <h2 className="hero-text text-4xl md:text-5xl mt-2 mb-4">
                        Simple, predictable<br />
                        <span className="text-secondary font-light">pricing.</span>
                    </h2>
                    <p className="text-secondary text-lg max-w-lg mx-auto mb-14">
                        Start free. Scale as you grow. No surprise bills.
                    </p>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-5 text-left">
                        {[
                            {
                                plan: "Free", price: "$0", period: "/ month",
                                desc: "For individuals and side projects.",
                                features: ["2,000 console notifications", "150 emails / month", "1 Slack & Discord channel"],
                                cta: "Start for free", primary: false
                            },
                            {
                                plan: "Pro", price: "$29", period: "/ month",
                                desc: "For growing teams with real users.",
                                features: ["10,000 console notifications", "500 emails / month", "250 SMS / month", "Unlimited Slack & Discord"],
                                cta: "Get Pro", primary: true
                            },
                            {
                                plan: "Enterprise", price: "Custom", period: "",
                                desc: "For large-scale deployments.",
                                features: ["Unlimited notifications", "Custom email limits", "1,000+ SMS / month", "24/7 priority support"],
                                cta: "Contact sales", primary: false
                            }
                        ].map(tier => (
                            <div key={tier.plan} className={`card p-7 flex flex-col gap-5 ${tier.primary ? "border-accent-primary/30 shadow-accent" : ""}`}>
                                <div>
                                    <p className="text-xs font-bold uppercase tracking-widest text-tertiary mb-2">{tier.plan}</p>
                                    <div className="flex items-end gap-1">
                                        <span className="font-display font-bold text-4xl text-primary">{tier.price}</span>
                                        {tier.period && <span className="text-secondary text-sm mb-1.5">{tier.period}</span>}
                                    </div>
                                    <p className="text-sm text-secondary mt-1.5">{tier.desc}</p>
                                </div>
                                <ul className="space-y-2.5 flex-1">
                                    {tier.features.map(f => (
                                        <li key={f} className="flex items-center gap-2.5 text-sm text-secondary">
                                            <svg className="w-4 h-4 text-green-500 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                            </svg>
                                            {f}
                                        </li>
                                    ))}
                                </ul>
                                <button
                                    onClick={() => navigate(tier.plan === "Enterprise" ? "/contact" : "/register")}
                                    className={tier.primary ? "btn-primary w-full py-3" : "btn-secondary w-full py-3"}
                                >
                                    {tier.cta}
                                </button>
                            </div>
                        ))}
                    </div>
                    <p className="text-xs text-tertiary mt-6">
                        All plans include API access, real-time stats, and delivery logs.{" "}
                        <button onClick={() => navigate("/pricing")} className="text-accent-primary hover:underline">
                            See full comparison →
                        </button>
                    </p>
                </div>
            </section>

            {/* ══════════════════════════════════════════════════════════════════════ */}
            {/* CTA BANNER                                                            */}
            {/* ══════════════════════════════════════════════════════════════════════ */}
            <section className="py-32 px-6 relative overflow-hidden">
                <div className="absolute inset-0 glow-overlay-center" />
                <div className="max-w-3xl mx-auto text-center relative z-10">
                    <h2 className="hero-text text-4xl md:text-6xl mb-5">
                        Start sending<br />
                        <span className="gradient-text">in under 5 minutes.</span>
                    </h2>
                    <p className="text-secondary text-lg mb-10 max-w-lg mx-auto">
                        Join hundreds of developers who trust Fertile Notify to power their customer communication.
                        No complex setup. No hidden fees.
                    </p>
                    <div className="flex flex-col sm:flex-row gap-3.5 justify-center">
                        <button
                            onClick={() => navigate(isAuthenticated ? "/dashboard" : "/register")}
                            className="btn-primary px-9 py-3.5 text-[15px] shadow-accent-lg"
                        >
                            {isAuthenticated ? "Open Dashboard" : "Create free account"}
                        </button>
                    </div>
                </div>
            </section>

            {/* ══════════════════════════════════════════════════════════════════════ */}
            {/* FOOTER                                                                */}
            {/* ══════════════════════════════════════════════════════════════════════ */}
            <footer className="border-t border-primary py-16 px-6">
                <div className="max-w-7xl mx-auto">
                    <div className="flex flex-col md:flex-row justify-between gap-12">
                        <div className="md:w-56">
                            <div className="flex items-center gap-2 mb-4">
                                <div className="w-6 h-6 bg-white rounded flex items-center justify-center">
                                    <span className="text-black font-bold text-[10px] font-mono">FN</span>
                                </div>
                                <span className="font-display font-bold text-sm">
                                    fertile<span className="text-accent-primary">notify</span>
                                </span>
                            </div>
                            <p className="text-xs text-tertiary leading-relaxed">
                                Unified notification infrastructure for modern development teams.
                            </p>
                        </div>

                        <div className="grid grid-cols-2 md:grid-cols-2 gap-10">
                            <div>
                                <p className="text-[10px] font-bold uppercase tracking-widest text-muted mb-4">Product</p>
                                <div className="flex flex-col gap-2.5">
                                    {[
                                        { label: "Features",     action: () => document.getElementById("features")?.scrollIntoView({ behavior: "smooth" }) },
                                        { label: "API Reference", action: () => navigate("/api-reference") },
                                        { label: "Changelog",    action: () => navigate("/changelog") },
                                        { label: "Documentation", action: () => navigate("/documentation") },
                                    ].map(l => (
                                        <button key={l.label} onClick={l.action} className="text-left text-sm text-tertiary hover:text-primary transition-colors">
                                            {l.label}
                                        </button>
                                    ))}
                                </div>
                            </div>
                            <div>
                                <p className="text-[10px] font-bold uppercase tracking-widest text-muted mb-4">Company</p>
                                <div className="flex flex-col gap-2.5">
                                    {[
                                        { label: "About Us", action: () => navigate("/about") },
                                        { label: "Contact",  action: () => navigate("/contact") },
                                        { label: "Privacy",  action: () => navigate("/privacy") },
                                        { label: "Terms",    action: () => navigate("/terms") },
                                    ].map(l => (
                                        <button key={l.label} onClick={l.action} className="text-left text-sm text-tertiary hover:text-primary transition-colors">
                                            {l.label}
                                        </button>
                                    ))}
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="mt-12 pt-8 border-t border-primary flex flex-col md:flex-row justify-between items-center gap-4">
                        <p className="text-[11px] text-muted">
                            © {new Date().getFullYear()} Fertile Notify. All rights reserved.
                        </p>
                        <div className="flex items-center gap-1">
                            <span className="w-1.5 h-1.5 rounded-full bg-green-500 animate-pulse" />
                            <span className="text-[11px] text-tertiary">All systems operational</span>
                        </div>
                    </div>
                </div>
            </footer>
        </div>
    );
}
