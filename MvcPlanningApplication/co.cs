//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcPlanningApplication
{
    using System;
    using System.Collections.Generic;
    
    public partial class co
    {
        public string type { get; set; }
        public string co_num { get; set; }
        public string est_num { get; set; }
        public string cust_num { get; set; }
        public Nullable<int> cust_seq { get; set; }
        public string contact { get; set; }
        public string phone { get; set; }
        public string cust_po { get; set; }
        public System.DateTime order_date { get; set; }
        public string taken_by { get; set; }
        public string terms_code { get; set; }
        public string ship_code { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> weight { get; set; }
        public Nullable<short> qty_packages { get; set; }
        public Nullable<decimal> freight { get; set; }
        public Nullable<decimal> misc_charges { get; set; }
        public Nullable<decimal> prepaid_amt { get; set; }
        public Nullable<decimal> sales_tax { get; set; }
        public string stat { get; set; }
        public Nullable<decimal> cost { get; set; }
        public Nullable<System.DateTime> close_date { get; set; }
        public Nullable<decimal> freight_t { get; set; }
        public Nullable<decimal> m_charges_t { get; set; }
        public Nullable<decimal> prepaid_t { get; set; }
        public Nullable<decimal> sales_tax_t { get; set; }
        public string slsman { get; set; }
        public Nullable<System.DateTime> eff_date { get; set; }
        public Nullable<System.DateTime> exp_date { get; set; }
        public string whse { get; set; }
        public Nullable<decimal> sales_tax_2 { get; set; }
        public Nullable<decimal> sales_tax_t2 { get; set; }
        public Nullable<byte> edi_order { get; set; }
        public string trans_nat { get; set; }
        public string process_ind { get; set; }
        public string delterm { get; set; }
        public Nullable<byte> use_exch_rate { get; set; }
        public string tax_code1 { get; set; }
        public string tax_code2 { get; set; }
        public string frt_tax_code1 { get; set; }
        public string frt_tax_code2 { get; set; }
        public string msc_tax_code1 { get; set; }
        public string msc_tax_code2 { get; set; }
        public string discount_type { get; set; }
        public Nullable<decimal> disc_amount { get; set; }
        public Nullable<decimal> disc { get; set; }
        public string pricecode { get; set; }
        public Nullable<byte> ship_partial { get; set; }
        public Nullable<byte> ship_early { get; set; }
        public Nullable<decimal> matl_cost_t { get; set; }
        public Nullable<decimal> lbr_cost_t { get; set; }
        public Nullable<decimal> fovhd_cost_t { get; set; }
        public Nullable<decimal> vovhd_cost_t { get; set; }
        public Nullable<decimal> out_cost_t { get; set; }
        public string end_user_type { get; set; }
        public Nullable<decimal> exch_rate { get; set; }
        public Nullable<byte> fixed_rate { get; set; }
        public string orig_site { get; set; }
        public string lcr_num { get; set; }
        public string edi_type { get; set; }
        public Nullable<byte> invoiced { get; set; }
        public Nullable<byte> credit_hold { get; set; }
        public Nullable<System.DateTime> credit_hold_date { get; set; }
        public string credit_hold_reason { get; set; }
        public string credit_hold_user { get; set; }
        public Nullable<byte> sync_reqd { get; set; }
        public Nullable<System.DateTime> projected_date { get; set; }
        public string order_source { get; set; }
        public string convert_type { get; set; }
        public Nullable<byte> aps_pull_up { get; set; }
        public Nullable<byte> consolidate { get; set; }
        public string inv_freq { get; set; }
        public Nullable<byte> summarize { get; set; }
        public byte NoteExistsFlag { get; set; }
        public System.DateTime RecordDate { get; set; }
        public System.Guid RowPointer { get; set; }
        public Nullable<byte> einvoice { get; set; }
        public string charfld1 { get; set; }
        public string charfld2 { get; set; }
        public string charfld3 { get; set; }
        public Nullable<System.DateTime> datefld { get; set; }
        public Nullable<decimal> decifld1 { get; set; }
        public Nullable<decimal> decifld2 { get; set; }
        public Nullable<decimal> decifld3 { get; set; }
        public Nullable<byte> logifld { get; set; }
        public string ack_stat { get; set; }
        public string config_id { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public byte InWorkflow { get; set; }
        public Nullable<byte> include_tax_in_price { get; set; }
        public string trans_nat_2 { get; set; }
        public string apply_to_inv_num { get; set; }
        public string export_type { get; set; }
        public string external_confirmation_ref { get; set; }
        public byte is_external { get; set; }
        public string prospect_id { get; set; }
        public string opp_id { get; set; }
        public string lead_id { get; set; }
        public Nullable<short> days_shipped_before_due_date_tolerance { get; set; }
        public Nullable<short> days_shipped_after_due_date_tolerance { get; set; }
        public Nullable<decimal> shipped_over_ordered_qty_tolerance { get; set; }
        public Nullable<decimal> shipped_under_ordered_qty_tolerance { get; set; }
        public byte consignment { get; set; }
        public Nullable<short> priority { get; set; }
        public string uf_pref_type { get; set; }
        public string demanding_site { get; set; }
        public string demanding_site_po_num { get; set; }
    }
}
