﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Devart Entity Developer tool using Data Transfer Object template.
// Code is generated on: 7/1/2024 2:59:24 AM
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace PayItGlobal.DTOs.Generated
{

    public static partial class PayOnlinePortalConverter
    {

        public static PayOnlinePortalDto ToDto(this PayItGlobal.Domain.Entities.PayOnlinePortal source)
        {
            return source.ToDtoWithRelated(0);
        }

        public static PayOnlinePortalDto ToDtoWithRelated(this PayItGlobal.Domain.Entities.PayOnlinePortal source, int level)
        {
            if (source == null)
              return null;

            var target = new PayOnlinePortalDto();

            // Properties
            target.PayOnlinePortalID = source.PayOnlinePortalID;
            target.Title = source.Title;
            target.AddedDate = source.AddedDate;
            target.EditDate = source.EditDate;
            target.AdminUserID = source.AdminUserID;
            target.HeaderColor = source.HeaderColor;
            target.LogoWidth = source.LogoWidth;
            target.LogoHeight = source.LogoHeight;
            target.BGTextColor = source.BGTextColor;
            target.RefundPolicy = source.RefundPolicy;
            target.UsesNameOfBusiness = source.UsesNameOfBusiness;
            target.UsesInvoiceNumber = source.UsesInvoiceNumber;
            target.UsesDynamicField = source.UsesDynamicField;
            target.DynamicFieldTitle = source.DynamicFieldTitle;
            target.DynamicFieldRequired = source.DynamicFieldRequired;
            target.UsesDynamicField2 = source.UsesDynamicField2;
            target.DynamicField2Title = source.DynamicField2Title;
            target.DynamicField2Required = source.DynamicField2Required;
            target.URLSlug = source.URLSlug;
            target.ClientID = source.ClientID;
            target.PortalTypeID = source.PortalTypeID;
            target.HomepageInstructions = source.HomepageInstructions;
            target.PaymentPageInstructions = source.PaymentPageInstructions;
            target.ReceiptExtraText = source.ReceiptExtraText;
            target.PaymentTypesAllowed = source.PaymentTypesAllowed;
            target.UsesInvoices = source.UsesInvoices;
            target.BannerText = source.BannerText;
            target.UseMasterCard = source.UseMasterCard;
            target.UseVisa = source.UseVisa;
            target.UseAmex = source.UseAmex;
            target.UsesLoyaltyProgram = source.UsesLoyaltyProgram;
            target.CSCOptional = source.CSCOptional;
            target.UseAnyDaySubscriptions = source.UseAnyDaySubscriptions;
            target.UseWeeklySubscriptions = source.UseWeeklySubscriptions;
            target.UseDelayPay = source.UseDelayPay;
            target.Processor = source.Processor;
            target.ACHProcessor = source.ACHProcessor;
            target.BackgroundImage = source.BackgroundImage;
            target.PublicSitePayContainerHeader = source.PublicSitePayContainerHeader;
            target.UseDiscover = source.UseDiscover;
            target.UseSplitPay = source.UseSplitPay;
            target.SendARReceiptToCustomer = source.SendARReceiptToCustomer;
            target.SendARReceiptToClient = source.SendARReceiptToClient;
            target.SendPublicReceiptToClient = source.SendPublicReceiptToClient;
            target.UseNagMail = source.UseNagMail;
            target.ReceiptTitle = source.ReceiptTitle;
            target.UseQBNames = source.UseQBNames;
            target.UseClientEmailForReceipt = source.UseClientEmailForReceipt;
            target.ClassIDToFilterFor = source.ClassIDToFilterFor;
            target.ReceiptEmail = source.ReceiptEmail;
            target.UsesCustomAmountTabulation = source.UsesCustomAmountTabulation;
            target.CustomAmountText = source.CustomAmountText;
            target.ECommTheme = source.ECommTheme;
            target.LastTsysPwdChg = source.LastTsysPwdChg;
            target.ACHPayonlineProcessorID = source.ACHPayonlineProcessorID;
            target.CCPayonlineProcessorID = source.CCPayonlineProcessorID;
            target.RetailPayonlineProcessorID = source.RetailPayonlineProcessorID;
            target.FailEmail = source.FailEmail;
            target.Active = source.Active;
            target.ConvFeeExclude = source.ConvFeeExclude;
            target.InvoiceNumberRequired = source.InvoiceNumberRequired;
            target.AdobeSignAbbreviation = source.AdobeSignAbbreviation;
            target.LocationId = source.LocationId;
            target.ProcessACHAsCCD = source.ProcessACHAsCCD;

            // Navigation Properties
            if (level > 0) {
              target.PayonlineMerchantCredentials = source.PayonlineMerchantCredentials.ToDtosWithRelated(level - 1);
              target.PayonlinePayments = source.PayonlinePayments.ToDtosWithRelated(level - 1);
              target.PayonlineClient = source.PayonlineClient.ToDtoWithRelated(level - 1);
              target.PayonlinePortaltype = source.PayonlinePortaltype.ToDtoWithRelated(level - 1);
              target.User = source.User.ToDtoWithRelated(level - 1);
              target.ACHPayOnlineProcessor = source.ACHPayOnlineProcessor.ToDtoWithRelated(level - 1);
              target.CCPayOnlineProcessor = source.CCPayOnlineProcessor.ToDtoWithRelated(level - 1);
              target.RetailPayonlineProcessor = source.RetailPayonlineProcessor.ToDtoWithRelated(level - 1);
            }

            // User-defined partial method
            OnDtoCreating(source, target);

            return target;
        }

        public static PayItGlobal.Domain.Entities.PayOnlinePortal ToEntity(this PayOnlinePortalDto source)
        {
            if (source == null)
              return null;

            var target = new PayItGlobal.Domain.Entities.PayOnlinePortal();

            // Properties
            target.PayOnlinePortalID = source.PayOnlinePortalID;
            target.Title = source.Title;
            target.AddedDate = source.AddedDate;
            target.EditDate = source.EditDate;
            target.AdminUserID = source.AdminUserID;
            target.HeaderColor = source.HeaderColor;
            target.LogoWidth = source.LogoWidth;
            target.LogoHeight = source.LogoHeight;
            target.BGTextColor = source.BGTextColor;
            target.RefundPolicy = source.RefundPolicy;
            target.UsesNameOfBusiness = source.UsesNameOfBusiness;
            target.UsesInvoiceNumber = source.UsesInvoiceNumber;
            target.UsesDynamicField = source.UsesDynamicField;
            target.DynamicFieldTitle = source.DynamicFieldTitle;
            target.DynamicFieldRequired = source.DynamicFieldRequired;
            target.UsesDynamicField2 = source.UsesDynamicField2;
            target.DynamicField2Title = source.DynamicField2Title;
            target.DynamicField2Required = source.DynamicField2Required;
            target.URLSlug = source.URLSlug;
            target.ClientID = source.ClientID;
            target.PortalTypeID = source.PortalTypeID;
            target.HomepageInstructions = source.HomepageInstructions;
            target.PaymentPageInstructions = source.PaymentPageInstructions;
            target.ReceiptExtraText = source.ReceiptExtraText;
            target.PaymentTypesAllowed = source.PaymentTypesAllowed;
            target.UsesInvoices = source.UsesInvoices;
            target.BannerText = source.BannerText;
            target.UseMasterCard = source.UseMasterCard;
            target.UseVisa = source.UseVisa;
            target.UseAmex = source.UseAmex;
            target.UsesLoyaltyProgram = source.UsesLoyaltyProgram;
            target.CSCOptional = source.CSCOptional;
            target.UseAnyDaySubscriptions = source.UseAnyDaySubscriptions;
            target.UseWeeklySubscriptions = source.UseWeeklySubscriptions;
            target.UseDelayPay = source.UseDelayPay;
            target.Processor = source.Processor;
            target.ACHProcessor = source.ACHProcessor;
            target.BackgroundImage = source.BackgroundImage;
            target.PublicSitePayContainerHeader = source.PublicSitePayContainerHeader;
            target.UseDiscover = source.UseDiscover;
            target.UseSplitPay = source.UseSplitPay;
            target.SendARReceiptToCustomer = source.SendARReceiptToCustomer;
            target.SendARReceiptToClient = source.SendARReceiptToClient;
            target.SendPublicReceiptToClient = source.SendPublicReceiptToClient;
            target.UseNagMail = source.UseNagMail;
            target.ReceiptTitle = source.ReceiptTitle;
            target.UseQBNames = source.UseQBNames;
            target.UseClientEmailForReceipt = source.UseClientEmailForReceipt;
            target.ClassIDToFilterFor = source.ClassIDToFilterFor;
            target.ReceiptEmail = source.ReceiptEmail;
            target.UsesCustomAmountTabulation = source.UsesCustomAmountTabulation;
            target.CustomAmountText = source.CustomAmountText;
            target.ECommTheme = source.ECommTheme;
            target.LastTsysPwdChg = source.LastTsysPwdChg;
            target.ACHPayonlineProcessorID = source.ACHPayonlineProcessorID;
            target.CCPayonlineProcessorID = source.CCPayonlineProcessorID;
            target.RetailPayonlineProcessorID = source.RetailPayonlineProcessorID;
            target.FailEmail = source.FailEmail;
            target.Active = source.Active;
            target.ConvFeeExclude = source.ConvFeeExclude;
            target.InvoiceNumberRequired = source.InvoiceNumberRequired;
            target.AdobeSignAbbreviation = source.AdobeSignAbbreviation;
            target.LocationId = source.LocationId;
            target.ProcessACHAsCCD = source.ProcessACHAsCCD;

            // User-defined partial method
            OnEntityCreating(source, target);

            return target;
        }

        public static List<PayOnlinePortalDto> ToDtos(this IEnumerable<PayItGlobal.Domain.Entities.PayOnlinePortal> source)
        {
            if (source == null)
              return null;

            var target = source
              .Select(src => src.ToDto())
              .ToList();

            return target;
        }

        public static List<PayOnlinePortalDto> ToDtosWithRelated(this IEnumerable<PayItGlobal.Domain.Entities.PayOnlinePortal> source, int level)
        {
            if (source == null)
              return null;

            var target = source
              .Select(src => src.ToDtoWithRelated(level))
              .ToList();

            return target;
        }

        public static List<PayItGlobal.Domain.Entities.PayOnlinePortal> ToEntities(this IEnumerable<PayOnlinePortalDto> source)
        {
            if (source == null)
              return null;

            var target = source
              .Select(src => src.ToEntity())
              .ToList();

            return target;
        }

        static partial void OnDtoCreating(PayItGlobal.Domain.Entities.PayOnlinePortal source, PayOnlinePortalDto target);

        static partial void OnEntityCreating(PayOnlinePortalDto source, PayItGlobal.Domain.Entities.PayOnlinePortal target);

    }

}
