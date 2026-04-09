// Domain
global using FertileNotify.Domain.Exceptions;
global using FertileNotify.Domain.ValueObjects;
global using FertileNotify.Domain.Entities;
global using FertileNotify.Domain.Enums;

// Application - Interfaces
global using FertileNotify.Application.Interfaces.Automation;
global using FertileNotify.Application.Interfaces.Notifications;
global using FertileNotify.Application.Interfaces.Observability;
global using FertileNotify.Application.Interfaces.Security;
global using FertileNotify.Application.Interfaces.Subscribers;

// Application - Services
global using FertileNotify.Application.Services.Automation;
global using FertileNotify.Application.Services.Notifications;
global using FertileNotify.Application.Services.Observability;
global using FertileNotify.Application.Services.Security;

// Application - DTOs
global using FertileNotify.Application.DTOs.Notifications;
global using FertileNotify.Application.DTOs.Security;
global using FertileNotify.Application.DTOs.Subscribers;

// Application - Use Cases
global using FertileNotify.Application.UseCases.ForgotPassword;
global using FertileNotify.Application.UseCases.Login;
global using FertileNotify.Application.UseCases.RefreshToken;
global using FertileNotify.Application.UseCases.VerifyCode;
global using FertileNotify.Application.UseCases.SystemNotification;
global using FertileNotify.Application.UseCases.SendNotification;
global using FertileNotify.Application.UseCases.Workflow;
global using FertileNotify.Application.UseCases.NotificationComplaint;
global using FertileNotify.Application.UseCases.Unsubscribe;
global using FertileNotify.Application.UseCases.CreateApiKey;
global using FertileNotify.Application.UseCases.DeleteAccount;
global using FertileNotify.Application.UseCases.ExportData;
global using FertileNotify.Application.UseCases.ManageChannels;
global using FertileNotify.Application.UseCases.RegisterSubscriber;
global using FertileNotify.Application.UseCases.RevokeApiKey;
global using FertileNotify.Application.UseCases.SetChannelSetting;
global using FertileNotify.Application.UseCases.UpdateCompanyName;
global using FertileNotify.Application.UseCases.UpdateContactInfo;
global using FertileNotify.Application.UseCases.UpdatePassword;
global using FertileNotify.Application.UseCases.Templates;

// Infrastructure
global using FertileNotify.Infrastructure.BackgroundJobs.Automation;
global using FertileNotify.Infrastructure.BackgroundJobs.Notifications;
global using FertileNotify.Infrastructure.BackgroundJobs.Observability;

// API
global using FertileNotify.API.Models.Requests;
global using FertileNotify.API.Models.Responses;

// Third Party & System
global using MediatR;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Json;
