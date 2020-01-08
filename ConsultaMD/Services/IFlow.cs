using ConsultaMD.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public interface IFlow
    {
        FlowSettings FlowSettings { get; set; }
        string GetSignature(string sortedParams);
        byte[] StringEncode(string text);
        SortedDictionary<string, string> Sign(SortedDictionary<string, string> ccForm);
        Task<Customer> CustomerCreate(string email, int id);
        Task<Register> CustomerRegister(string customerId, Uri returnUrl);
        Task<Customer> GetRegisterStatus(string token);
        Task<Customer> CustomerCharge(string token);
    }
}
