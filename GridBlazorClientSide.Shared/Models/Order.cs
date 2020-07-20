//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GridShared.DataAnnotations;
using GridShared.Sorting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GridBlazorClientSide.Shared.Models
{

    public class OrderMetaData
    {
        [Display(Name = "Ship name", AutoGenerateFilter = true)]
        public string ShipName { get; set; }
    }

    [GridMetadataTypeAttribute(typeof(OrderMetaData))]
    //[GridTable(PagingEnabled = true, PageSize = 20)]
    public partial class Order
    {
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }
        [Key]
        [GridHiddenColumn]
        [Required]
        [GridColumn(Position = 0)]
        public int OrderID { get; set; }
        [GridColumn(Position = 5)]
        [Required]
        public string CustomerID { get; set; }
        [GridColumn(Position = 4)]
        [Required]
        public int? EmployeeID { get; set; }
        [GridColumn(Position = 1, Title = "Date", Width = "120px", Format = "{0:yyyy-MM-dd}", SortEnabled = true, FilterEnabled = true, SortInitialDirection = GridSortDirection.Ascending)]
        public DateTime? OrderDate { get; set; }
        [GridColumn(Position = 2, Width = "120px", Format = "{0:yyyy-MM-dd}")]
        public DateTime? RequiredDate { get; set; }
        [GridColumn(Position = 3, Width = "120px", Format = "{0:yyyy-MM-dd}")]
        public DateTime? ShippedDate { get; set; }
        [GridColumn(Position = 6)]
        public int? ShipVia { get; set; }
        [GridColumn(Position = 7, Title = "Freight", Width = "120px", SortEnabled = true, FilterEnabled = true)]
        public decimal? Freight { get; set; }
        [GridColumn(Position = 8)]
        public string ShipName { get; set; }
        [GridColumn(Position = 9)]
        public string ShipAddress { get; set; }
        [GridColumn(Position = 10)]
        public string ShipCity { get; set; }
        [GridColumn(Position = 11)]
        public string ShipRegion { get; set; }
        [GridColumn(Position = 12)]
        public string ShipPostalCode { get; set; }
        [GridColumn(Position = 13)]
        public string ShipCountry { get; set; }
        [ForeignKey("CustomerID")]
        [GridColumn(Position = 14)]
        [NotMappedColumn]
        public virtual Customer Customer { get; set; }
        [GridColumn(Position = 15)]
        [NotMappedColumn]
        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }
        [JsonIgnore]
        [GridColumn(Position = 17)]
        [NotMappedColumn]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [GridColumn(Position = 16)]
        [NotMappedColumn]
        [ForeignKey("ShipVia")]
        public virtual Shipper Shipper { get; set; }
    }

}
