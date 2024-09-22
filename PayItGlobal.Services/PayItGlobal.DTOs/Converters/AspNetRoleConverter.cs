﻿//------------------------------------------------------------------------------
// This is auto-generated code.
//------------------------------------------------------------------------------
// This code was generated by Devart Entity Developer tool using Data Transfer Object template.
// Code is generated on: 7/11/2024 6:53:55 PM
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace PayItGlobalApi.DTOs.Generated
{

    public static partial class AspNetRoleConverter
    {

        public static AspNetRoleDto ToDto(this PayItGlobalApi.Domain.Entities.AspNetRole source)
        {
            return source.ToDtoWithRelated(0);
        }

        public static AspNetRoleDto ToDtoWithRelated(this PayItGlobalApi.Domain.Entities.AspNetRole source, int level)
        {
            if (source == null)
              return null;

            var target = new AspNetRoleDto();

            // Properties
            target.Id = source.Id;
            target.Name = source.Name;
            target.NormalizedName = source.NormalizedName;
            target.ConcurrencyStamp = source.ConcurrencyStamp;

            // Navigation Properties
            if (level > 0) {
              target.AspNetUsers = source.AspNetUsers.ToDtosWithRelated(level - 1);
              target.AspNetRoleClaims = source.AspNetRoleClaims.ToDtosWithRelated(level - 1);
            }

            // User-defined partial method
            OnDtoCreating(source, target);

            return target;
        }

        public static PayItGlobalApi.Domain.Entities.AspNetRole ToEntity(this AspNetRoleDto source)
        {
            if (source == null)
              return null;

            var target = new PayItGlobalApi.Domain.Entities.AspNetRole();

            // Properties
            target.Id = source.Id;
            target.Name = source.Name;
            target.NormalizedName = source.NormalizedName;
            target.ConcurrencyStamp = source.ConcurrencyStamp;

            // User-defined partial method
            OnEntityCreating(source, target);

            return target;
        }

        public static List<AspNetRoleDto> ToDtos(this IEnumerable<PayItGlobalApi.Domain.Entities.AspNetRole> source)
        {
            if (source == null)
              return null;

            var target = source
              .Select(src => src.ToDto())
              .ToList();

            return target;
        }

        public static List<AspNetRoleDto> ToDtosWithRelated(this IEnumerable<PayItGlobalApi.Domain.Entities.AspNetRole> source, int level)
        {
            if (source == null)
              return null;

            var target = source
              .Select(src => src.ToDtoWithRelated(level))
              .ToList();

            return target;
        }

        public static List<PayItGlobalApi.Domain.Entities.AspNetRole> ToEntities(this IEnumerable<AspNetRoleDto> source)
        {
            if (source == null)
              return null;

            var target = source
              .Select(src => src.ToEntity())
              .ToList();

            return target;
        }

        static partial void OnDtoCreating(PayItGlobalApi.Domain.Entities.AspNetRole source, AspNetRoleDto target);

        static partial void OnEntityCreating(AspNetRoleDto source, PayItGlobalApi.Domain.Entities.AspNetRole target);

    }

}
