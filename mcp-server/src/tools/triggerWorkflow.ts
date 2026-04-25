import { backendClient } from "../backendClient.js";
import { API_KEY } from "../config.js";

export const triggerWorkflowTool = {
  name: "trigger_workflow",
  description: "Trigger a workflow through FertileNotify backend (/api/notifications/workflow/send/{eventTrigger}).",
  inputSchema: {
    type: "object",
    properties: {
      eventTrigger: {
        type: "string",
        description: "Workflow event trigger configured in backend, for example user_signup.",
      },
    },
    required: ["eventTrigger"],
  },
} as const;

export async function handleTriggerWorkflowTool(argumentsValue: unknown) {
  if (!API_KEY) {
    return {
      content: [{ type: "text", text: "Missing FERTILE_NOTIFY_API_KEY. Set it before starting MCP server." }],
      isError: true,
    };
  }

  const args = (argumentsValue ?? {}) as {
    eventTrigger?: string;
  };

  if (!args.eventTrigger || !args.eventTrigger.trim()) {
    return {
      content: [{ type: "text", text: "Invalid input. Required: eventTrigger." }],
      isError: true,
    };
  }

  try {
    const response = await backendClient.post(`/notifications/workflow/send/${encodeURIComponent(args.eventTrigger.trim())}`);
    const count = response.data?.data?.count;

    return {
      content: [{
        type: "text",
        text: count != null
          ? `Success. ${count} workflow notification(s) queued.`
          : "Success. Workflow trigger request accepted by backend.",
      }],
    };
  } catch (error: any) {
    const backendErrors = error.response?.data?.errors;
    const backendMessage = error.response?.data?.message;
    const details = Array.isArray(backendErrors) && backendErrors.length > 0
      ? backendErrors.join(" | ")
      : backendMessage;

    return {
      content: [{ type: "text", text: `Error occurred: ${details || error.message}` }],
      isError: true,
    };
  }
}
