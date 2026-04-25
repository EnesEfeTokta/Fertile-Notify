import { backendClient } from "../backendClient.js";
import { API_KEY } from "../config.js";

export const sendNotificationTool = {
  name: "send_notification",
  description: "Send a notification through FertileNotify backend (/api/notifications/send).",
  inputSchema: {
    type: "object",
    properties: {
      eventType: {
        type: "string",
        description: "Template event type defined in backend, for example TestForDevelop.",
      },
      channel: {
        type: "string",
        enum: [
          "email",
          "sms",
          "whatsapp",
          "telegram",
          "discord",
          "slack",
          "webhook",
          "webpush",
          "msteams",
          "console",
          "firebasepush",
        ],
        description: "Target notification channel.",
      },
      recipients: {
        type: "array",
        items: { type: "string" },
        minItems: 1,
        description: "One or more recipients for the selected channel.",
      },
      parameters: {
        type: "object",
        additionalProperties: { type: "string" },
        description: "Optional template parameters as key/value pairs.",
      },
    },
    required: ["eventType", "channel", "recipients"],
  },
} as const;

export async function handleSendNotificationTool(argumentsValue: unknown) {
  if (!API_KEY) {
    return {
      content: [{ type: "text", text: "Missing FERTILE_NOTIFY_API_KEY. Set it before starting MCP server." }],
      isError: true,
    };
  }

  const args = (argumentsValue ?? {}) as {
    eventType?: string;
    channel?: string;
    recipients?: string[];
    parameters?: Record<string, string>;
  };

  if (!args.eventType || !args.channel || !args.recipients || args.recipients.length === 0) {
    return {
      content: [{ type: "text", text: "Invalid input. Required: eventType, channel, recipients[]." }],
      isError: true,
    };
  }

  const payload = {
    eventType: args.eventType,
    parameters: args.parameters ?? {},
    to: [
      {
        channel: args.channel,
        recipients: args.recipients,
      },
    ],
  };

  try {
    const response = await backendClient.post("/notifications/send", payload);
    const count = response.data?.data?.count;

    return {
      content: [{
        type: "text",
        text: count != null
          ? `Success. ${count} notification(s) queued.`
          : "Success. Notification request accepted by backend.",
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
