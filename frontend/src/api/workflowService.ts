import axiosClient from './axiosClient';
import type { Workflow, CreateWorkflowRequest, UpdateWorkflowRequest } from '../types/workflow';

export const workflowService = {
    listWorkflows: async (): Promise<Workflow[]> => {
        const response = await axiosClient.get('notifications/workflow/list');
        return response.data;
    },

    getWorkflow: async (id: string): Promise<Workflow> => {
        const response = await axiosClient.get(`notifications/workflow/get/${id}`);
        return response.data;
    },

    addWorkflow: async (req: CreateWorkflowRequest): Promise<{ workflowId: string }> => {
        const response = await axiosClient.post('notifications/workflow/add', req);
        return response.data;
    },

    updateWorkflow: async (req: UpdateWorkflowRequest): Promise<void> => {
        await axiosClient.put('notifications/workflow/update', req);
    },

    deleteWorkflow: async (id: string): Promise<void> => {
        await axiosClient.delete(`notifications/workflow/delete/${id}`);
    },

    activateWorkflow: async (id: string): Promise<void> => {
        await axiosClient.post(`notifications/workflow/activate/${id}`);
    },

    deactivateWorkflow: async (id: string): Promise<void> => {
        await axiosClient.post(`notifications/workflow/deactivate/${id}`);
    },

    triggerWorkflow: async (eventTrigger: string): Promise<{ count: number }> => {
        const response = await axiosClient.post(`notifications/workflow/send/${encodeURIComponent(eventTrigger)}`);
        return response.data;
    },
};
