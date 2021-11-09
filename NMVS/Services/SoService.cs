using NMVS.Models;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public class SoService : ISoService
    {
        private readonly ApplicationDbContext _context;

        public SoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SoVm> GetBrowseData()
        {
            var model = (from so in _context.SalesOrders
                         join cust in _context.Customers on so.CustCode equals cust.CustCode into all

                         from a in all.DefaultIfEmpty()
                         join c in _context.Customers on so.ShipTo equals c.CustCode
                         select new SoVm
                         {
                             CustCode = a.CustName,
                             SoType = so.SoType,
                             ShipTo = c.CustName,
                             Comment = so.Comment,
                             Confirm = so.Confirm,
                             ConfirmBy = so.ConfirmBy,
                             DueDate = so.DueDate,
                             OrdDate = so.OrdDate,
                             PriceDate = so.PriceDate,
                             ReqDate = so.ReqDate,
                             ShipVia = so.ShipVia,
                             SoCurr = so.SoCurr,
                             SoNbr = so.SoNbr,
                             UpdatedBy = so.UpdatedBy,
                             UpdatedOn = so.UpdatedOn,
                             WhConfirmed = so.RequestConfirmed,
                             WhConfirmedBy = so.RequestConfirmedBy,
                             ConfirmationNote = so.ConfirmationNote,
                             Closed = so.Closed,
                             ReqReported = so.ReqReported,
                             ReqReportedNote = so.ReqReportedNote
                         }).ToList();

            return model;
        }

        public List<ItemAvailVm> GetItemNAvail()
        {
            List<ItemAvailVm> ls = new();
            var ptMstr = _context.ItemMasters.ToList();
            foreach (var item in _context.ItemDatas.ToList())
            {
                var pt = ptMstr.Where(x => x.ItemNo == item.ItemNo);
                double avail = 0;
                if (pt.Any())
                {
                    var hold = pt.Sum(x => x.PtHold);
                    var qty = pt.Sum(x => x.PtQty);
                    avail = qty - hold;
                }

                ls.Add(new ItemAvailVm
                {
                    ItemNo = item.ItemNo,
                    Quantity = avail,
                    Desc = item.ItemName
                });


            }
            return ls;
        }
        public SoDetailVm GetSoDetail(string soNbr)
        {
            var soVm = GetBrowseData().FirstOrDefault(x => x.SoNbr == soNbr);
            var soDets = _context.SoDetails.Where(x => x.SoNbr == soNbr).ToList();

            return new SoDetailVm
            {
                SoDets = soDets,
                SoVm = soVm
            };
        }

        public string GetSoNbr(string input, string soType)
        {
            string nbr = input[2..];
            string header = input.Substring(0, 2);
            if (soType == "Sale")
            {
                if (header == "WT" || header == "WR")
                {
                    return "SO" + nbr;
                }
                else if (header != "SO")
                {
                    return "SO" + input;
                }
                else
                {
                    return input;
                }
            }
            else if (soType == "WH Transfer")
            {
                if (header == "SO" || header == "WR")
                {
                    return "WT" + nbr;
                }
                else if (header != "WT")
                {
                    return "WT" + input;
                }
                else
                {
                    return input;
                }
            }
            else if (soType == "Warranty return")
            {
                if (header == "SO" || header == "WR")
                {
                    return "WR" + nbr;
                }
                else if (header != "WR")
                {
                    return "WR" + input;
                }
                else
                {
                    return input;
                }
            }
            else
            {
                return "";
            }
        }
    }
}
