﻿using System;
using System.Collections.Generic;
using System.Linq;
using Api.Domain.Enums;
using AutoMapper;
using FluentValidation;
using Infrastructure;
using MediatR;

namespace CMP.Features.Campaigns
{
    public class AddProductGroupToCampaign
    {
        public class Command : IRequest<Result>
        {
            public List<Guid> ProductGroupIds { get; set; }
            public Guid CampaignId { get; set; }
            public int Volume { get; set; }
            public int ApoVolume { get; set; }
            public decimal OnInvoiceValue { get; set; }
            public decimal OffInvoiceValue { get; set; }
            public int CurrencyValue { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ProductGroupIds).SetCollectionValidator(new GuidListValidator());
                RuleFor(x => x.CampaignId).NotEmpty();
                RuleFor(x => x.Volume).NotEmpty();
                RuleFor(x => x.ApoVolume).NotEmpty();
                RuleFor(x => x.OnInvoiceValue).NotEmpty();
                RuleFor(x => x.OffInvoiceValue).NotEmpty();
                RuleFor(x => x.CurrencyValue).NotEmpty();
            }
        }

        private class GuidListValidator : AbstractValidator<Guid>
        {
            public GuidListValidator()
            {
                RuleFor(guid => guid).NotEmpty();
            }
        }

        public class Result
        {
            public bool Success { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly CmpContext _context;
            private readonly IConfigurationProvider _config;

            public Handler(CmpContext context, IConfigurationProvider config)
            {
                _context = context;
                _config = config;
            }

            public Result Handle(Command command)
            {
                var campaign = _context.Campaigns.Single(c => c.Id == command.CampaignId);
                var products = _context.Products.Where(p => command.ProductGroupIds.Any(id => id == p.ProductGroup.Id));

                foreach (var product in products)
                {
                    campaign.AddProduct(product, command.Volume, command.ApoVolume, command.OnInvoiceValue, command.OffInvoiceValue, Enumeration.FromValue<Currency>(command.CurrencyValue));
                }

                _context.SaveChanges();

                return new Result { Success = true };
            }
        }
    }
}