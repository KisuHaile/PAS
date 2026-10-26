global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;

global using Application.Common.Interfaces;
global using Application.Common.Mappings;
global using Application.Common.Models;
global using Application.Common.Exceptions;
global using Application.Events;

global using Domain.Common;
global using Domain.Catalog;
global using Domain.Storage;
global using Domain.Users;
global using Domain.Receiving;
global using Domain.Requisition;
global using Domain.PropertyManagement;
global using Domain.Disposal;
global using Domain.TransferReturn;
global using Domain.Workflow;
global using MediatR;
global using AutoMapper;
global using AutoMapper.QueryableExtensions;

global using Application.Common.Security;
global using SystemRoles = Application.Constants.SystemRoles;
