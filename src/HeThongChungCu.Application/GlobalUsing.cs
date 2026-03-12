global using MediatR;
global using FluentValidation;

global using HeThongChungCu.Domain.Common;
global using HeThongChungCu.Domain.Enums;
global using HeThongChungCu.Domain.Errors;
global using HeThongChungCu.Domain.Entities.Identity;
global using HeThongChungCu.Domain.Entities.ChungCu;
global using HeThongChungCu.Domain.Entities.PhuongTien;

global using HeThongChungCu.Application.Common.Messaging;
global using HeThongChungCu.Application.Common.Models;
global using HeThongChungCu.Application.Common.Options;
global using HeThongChungCu.Application.Common.Interfaces.Services;
global using HeThongChungCu.Application.Common.Interfaces.Persistences.EF;
global using HeThongChungCu.Application.Common.Interfaces.Persistences.Dapper;
