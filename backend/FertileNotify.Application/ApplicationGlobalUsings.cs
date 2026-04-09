// DTOs
global using FertileNotify.Application.DTOs.Automation;
global using FertileNotify.Application.DTOs.Notifications;
global using FertileNotify.Application.DTOs.Observability;
global using FertileNotify.Application.DTOs.Security;
global using FertileNotify.Application.DTOs.Subscribers;

// Interfaces
global using FertileNotify.Application.Interfaces.Automation;
global using FertileNotify.Application.Interfaces.Notifications;
global using FertileNotify.Application.Interfaces.Observability;
global using FertileNotify.Application.Interfaces.Security;
global using FertileNotify.Application.Interfaces.Subscribers;

// Services
global using FertileNotify.Application.Services.Automation;
global using FertileNotify.Application.Services.Security;

// Use Cases & Contracts
global using FertileNotify.Application.UseCases.Common;
global using FertileNotify.Application.Contracts;

// Domain
global using FertileNotify.Domain.Entities;
global using FertileNotify.Domain.Enums;
global using FertileNotify.Domain.Events;
global using FertileNotify.Domain.Exceptions;
global using FertileNotify.Domain.ValueObjects;

// External Libraries
global using MediatR;
global using MassTransit;
global using Microsoft.Extensions.Logging;