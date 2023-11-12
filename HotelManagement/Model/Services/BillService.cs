﻿using HotelManagement.DTOs;
using HotelManagement.Utils;
using HotelManagement.ViewModel.StaffVM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Model.Services
{
    public class BillService
    {
        public BillService() { }
        private static BillService _ins;
        public static BillService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new BillService();
                }
                return _ins;
            }
            private set { _ins = value; }
        }

        public async Task<BillDTO> GetBillByRentalContract(string rentalContractId)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {

                    var rentalContract = await context.RentalContracts.FindAsync(rentalContractId);
                    var billDTO = new BillDTO
                    {
                        RentalContractId = rentalContract.RentalContractId,
                        RoomId = rentalContract.RoomId,
                        RoomNumber = (int)rentalContract.Room.RoomNumber,
                        RoomTypeName = rentalContract.Room.RoomType.RoomTypeName,
                        StartDate = rentalContract.StartDate,
                        EndDate = rentalContract.EndDate,
                        RoomPrice = rentalContract.Room.RoomType.Price,
                        ListListServicePayment = rentalContract.ProductUsings.Select(t => new ProductUsingDTO
                        {
                            RentalContractId = t.RentalContractId,
                            ProductId = t.ProductId,
                            ProductName = t.Product.ProductName,
                            UnitPrice = t.Product.Price,
                            Quantity = t.Quantity,
                        }).ToList()

                    };

                    var listService = billDTO.ListListServicePayment
                                                            .GroupBy(x => x.ProductId)
                                                            .Select(t => new ProductUsingDTO
                                                            {
                                                                RentalContractId = t.First().RentalContractId,
                                                                ProductId = t.First().ProductId,
                                                                ProductName = t.First().ProductName,
                                                                UnitPrice = t.First().UnitPrice,
                                                                Quantity = t.Sum(g => g.Quantity)
                                                            }).ToList();

                    billDTO.ListListServicePayment = listService;
                    return billDTO;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<(bool, string)> SaveBill(BillDTO bill)
        {
            try
            {
                using (var context = new HotelManagementEntities())
                {
                    var maxBillId = await context.Bills.MaxAsync(x=> x.BillId);
                    Bill newBill = new Bill
                    {
                        BillId = CreateNextBillId(maxBillId),
                        RentalContractId=bill.RentalContractId,
                        StaffId= bill.StaffId,
                        TotalPrice= bill.TotalPrice,
                        CreateDate= bill.CreateDate,    
                    };
                    context.Bills.Add(newBill);
                    RentalContract rental = await context.RentalContracts.FindAsync(bill.RentalContractId);
                    //rental.PersonNumber=rental.RoomCustomers.Count();
                    await context.SaveChangesAsync();
                    return (true, "Thanh toán thành công!");
                }
            }
            catch (Exception ex)
            {
                return (false, "Lỗi hệ thống");
            }
        }
        private string CreateNextBillId(string maxBillId)
        {
            if (maxBillId is null) return "HD001";
            int num = int.Parse(maxBillId.Substring(2));
            string nextNumString = (num + 1).ToString();
            while (nextNumString.Length <3) nextNumString = "0" + nextNumString;
            return "HD" + nextNumString;

        }
        //public async Task<List<BillDTO>> GetBillsByRentalContracts(List<string> rentalContractIds)
        //{
        //    try
        //    {
        //        using (var context = new HotelManagementEntities())
        //        {
        //            List<BillDTO> listBillDTO = new List<BillDTO>();
        //            foreach (var item in rentalContractIds)
        //            {
        //                var itemBillDTO = await (from r in context.Rooms
        //                                         join c in context.RentalContracts
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
      
        
      
    
        //public async Task<BillDTO> GetBillDetails(string id)
        //{
        //    try
        //    {
        //        using (var context = new HotelManagementEntities())
        //        {
        //            var b = await context.Bills.FindAsync(id);
                    
        //            BillDTO billdetail = new BillDTO{
        //                BillId = b.BillId,
        //                RentalContractId = b.RentalContractId,
        //                StaffId = b.StaffId,
        //                StaffName = b.S
        //                CustomerAddress = b.RentalContract.Customer.CustomerAddress,
        //                CustomerId = b.RentalContract.CustomerId,
        //                CustomerName = b.RentalContract.Customer.CustomerName,
        //                RoomId = b.RentalContract.RoomId,
        //                RoomNumber = (int)b.RentalContract.Room.RoomNumber,
        //                RoomTypeName = b.RentalContract.Room.RoomType.RoomTypeName,
        //                PersonNumber = (int)b.RentalContract.PersonNumber,
        //                RoomPrice = b.RentalContract.Room.RoomType.Price,
        //                NumberOfRentalDays = b.NumberOfRentalDays,
        //                ServicePrice = b.ServicePrice,
        //                TroublePrice = b.TroublePrice,
        //                TotalPrice = b.TotalPrice,
        //                DiscountPrice = b.DiscountPrice,
        //                Price = b.Price,
        //                StartDate = b.RentalContract.StartDate,
        //                CheckOutDate = b.RentalContract.CheckOutDate,
        //                StartTime = b.RentalContract.StartTime,
        //                CreateDate = b.CreateDate,
        //                ListListServicePayment = b.RentalContract.ServiceUsings.Select(t => new ProductUsingDTO
        //                {
        //                    RentalContractId = t.RentalContractId,
        //                    ServiceId = t.ServiceId,
        //                    ServiceName = t.Service.ServiceName,
        //                    ServiceType = t.Service.ServiceType,
        //                    UnitPrice = t.Service.ServicePrice,
        //                    Quantity = t.Quantity,
        //                }).ToList(),
        //                ListTroubleByCustomer = b.RentalContract.TroubleByCustomers.Select(t => new TroubleByCustomerDTO
        //                {
        //                    RentalContractId = t.RentalContractId,
        //                    TroubleId = t.TroubleId,
        //                    Title = t.Trouble.Title,
        //                    PredictedPrice = t.PredictedPrice,
        //                    Level = t.Trouble.Level,
        //                }).ToList()
        //            };
        //            return  billdetail;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
    }
}
