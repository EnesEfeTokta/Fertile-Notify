import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { CallToolRequestSchema, ListToolsRequestSchema } from "@modelcontextprotocol/sdk/types.js";
import axios from "axios";

const API_BASE_URL = "http://localhost:8080/api"; 

const server = new Server({
  name: "fertile-notify-mcp",
  version: "1.0.0",
}, {
  capabilities: { tools: {} },
});

server.setRequestHandler(ListToolsRequestSchema, async () => ({
  tools: [{
    name: "send_notification",
    description: "FertileNotify sends notifications via its system.",
    inputSchema: {
      type: "object",
      properties: {
        to: { type: "string", description: "Recipient information (Email, Phone number, etc.)" },
        content: { type: "string", description: "Message content" },
        channel: { type: "string", enum: ["Email", "SMS", "WhatsApp", "Telegram", "Discord", "Slack", "Webhook", "Webpush", "MsTeams", "Console", "FirebasePush"], description: "Channel to send the notification" }
      },
      required: ["to", "content", "channel"]
    }
  }]
}));

server.setRequestHandler(CallToolRequestSchema, async (request) => {
  if (request.params.name === "send_notification") {
    const { to, content, channel } = request.params.arguments as any;

    try {
      const response = await axios.post(`${API_BASE_URL}/notifications/send`, {
        receiver: to,
        body: content,
        type: channel
      });

      return {
        content: [{ type: "text", text: `Success! Notification ID: ${response.data.id}` }]
      };
    } catch (error: any) {
      return {
        content: [{ type: "text", text: `Error occurred: ${error.response?.data?.message || error.message}` }],
        isError: true
      };
    }
  }
  throw new Error("Tool not found");
});

const transport = new StdioServerTransport();
await server.connect(transport);
console.log("FertileNotify MCP Server is running...");