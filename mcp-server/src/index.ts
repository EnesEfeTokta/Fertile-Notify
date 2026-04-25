import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { CallToolRequestSchema, ListToolsRequestSchema } from "@modelcontextprotocol/sdk/types.js";
import { handleSendNotificationTool, handleTriggerWorkflowTool, sendNotificationTool, triggerWorkflowTool } from "./tools/index.js";

const server = new Server(
  {
    name: "fertile-notify-mcp",
    version: "1.0.0",
  },
  {
    capabilities: { tools: {} },
  }
);

server.setRequestHandler(ListToolsRequestSchema, async () => ({
  tools: [sendNotificationTool, triggerWorkflowTool],
}));

server.setRequestHandler(CallToolRequestSchema, async (request) => {
  if (request.params.name === "send_notification") {
    return await handleSendNotificationTool(request.params.arguments);
  }

  if (request.params.name === "trigger_workflow") {
    return await handleTriggerWorkflowTool(request.params.arguments);
  }

  throw new Error("Tool not found");
});

const transport = new StdioServerTransport();
await server.connect(transport);
console.log("FertileNotify MCP Server is running...");
