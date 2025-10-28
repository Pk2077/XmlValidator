using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using XmlValidation;
using XmlValidator.Extensions;
using XmlValidator.Models;
using static System.Net.Mime.MediaTypeNames;

namespace XmlValidator.Controllers
{
    public class ValidatorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Validates XML invoice files against the predefined validation rules
        /// </summary>
        /// <param name="PathToXml">Path to the XML file to be validated</param>
        /// <returns>JSON result containing validation errors or success message</returns>
        public ActionResult Validate(string PathToXml)
        {
            PathToXml = @"C:\Users\Admin\Desktop\testXml\1.xml";
            //@"C:\Users\Admin\Desktop\testXml\2.xml",
            //@"C:\Users\Admin\Desktop\testXml\3.xml",
            //@"C:\Users\Admin\Desktop\testXml\C11092867090_20240621T133433Z_JPCS-8.xml",
            //@"C:\Users\Admin\Desktop\testXml\1.1-Invoice-Sample.xml",
            //@"C:\Users\Admin\Desktop\testXml\1.1-Invoice-MultiLineItem-Sample.xml",
            var xmlFiles = GetXmlFiles(PathToXml);
            List<Rootobject> BaseXmls = new List<Rootobject>();
            List<ErrorModel> ValidationErrors = new List<ErrorModel>();
            string jsonText = string.Empty;
            foreach (var xmlFile in xmlFiles)
            {
                try
                {
                    string sourceFilePath = xmlFile;
                    string fileName = Path.GetFileName(sourceFilePath);
                    string destinationPath = Path.Combine(Server.MapPath("~/FilesForJson"), fileName);
                    System.IO.File.Copy(sourceFilePath, destinationPath, true);
                    XmlDocument xmlDoc = new XmlDocument();

                    using (Stream stream = new FileStream(destinationPath, FileMode.Open))
                    {
                        xmlDoc.Load(stream);
                    }
                    jsonText = JsonConvert.SerializeXmlNode(xmlDoc);
                    jsonText = new ValidationExtension().JsonHandler(jsonText);
                    Rootobject invoice = JsonConvert.DeserializeObject<Rootobject>(jsonText);
                    BaseXmls.Add(invoice);
                }
                catch (Exception ex)
                {
                    ErrorModel ErrorObj = new ErrorModel();
                    ErrorObj.errMsg = "XML format error " + ex.Message;
                    ValidationErrors.Add(ErrorObj);
                    return Json(new { errors = ValidationErrors });
                }
            }
            if (BaseXmls.Count > 0 && BaseXmls != null)
            {
                foreach (var item in BaseXmls)
                {
                    Invoice Invoice = item.Invoice;
                    string errorMsg = string.Empty;
                    if (Invoice != null)
                    {
                        #region Headervalidation/
                        List<ErrorModel> _Headervalidation = new List<ErrorModel>();
                        _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.ID, nameof(Invoice) + "Id", 26);
                        if (!string.IsNullOrWhiteSpace(Invoice.ID))
                        {
                            _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.ID, nameof(Invoice) + "Id"));
                        }
                        if (_Headervalidation.Count > 0)
                        {
                            foreach (var error in _Headervalidation)
                            {
                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                    ValidationErrors.Add(error);
                            }
                        }
                        _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.IssueDate, nameof(Invoice.IssueDate), 10);
                        if (!string.IsNullOrWhiteSpace(Invoice.IssueDate))
                        {
                            _Headervalidation.Add(new ValidationExtension().ValidateDateProperty(Invoice.IssueDate, nameof(Invoice.IssueDate)));
                        }
                        if (_Headervalidation.Count > 0)
                        {
                            foreach (var error in _Headervalidation)
                            {
                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                    ValidationErrors.Add(error);
                            }
                        }
                        _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.IssueTime, nameof(Invoice.IssueTime), 9);
                        if (!string.IsNullOrWhiteSpace(Invoice.IssueTime))
                        {
                            _Headervalidation.Add(new ValidationExtension().ValidateTimeProperty(Invoice.IssueTime, nameof(Invoice.IssueTime)));
                        }
                        if (_Headervalidation.Count > 0)
                        {
                            foreach (var error in _Headervalidation)
                            {
                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                    ValidationErrors.Add(error);
                            }
                        }
                        if (Invoice.InvoiceTypeCode != null)
                        {
                            _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.InvoiceTypeCode.listVersionID, nameof(Invoice.InvoiceTypeCode.listVersionID), 5);
                            if (!string.IsNullOrWhiteSpace(Invoice.InvoiceTypeCode.listVersionID))
                            {
                                _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.InvoiceTypeCode.listVersionID, nameof(Invoice.InvoiceTypeCode.listVersionID)));
                            }
                            if (_Headervalidation.Count > 0)
                            {
                                foreach (var error in _Headervalidation)
                                {
                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                        ValidationErrors.Add(error);
                                }
                            }
                            _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.InvoiceTypeCode.text, nameof(Invoice.InvoiceTypeCode), 2);
                            if (!string.IsNullOrWhiteSpace(Invoice.InvoiceTypeCode.text))
                            {
                                _Headervalidation.Add(new ValidationExtension().ValidateInvoiceType(Invoice.InvoiceTypeCode.text, nameof(Invoice.InvoiceTypeCode)));
                                _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.InvoiceTypeCode.text, nameof(Invoice.InvoiceTypeCode)));
                            }
                            if (_Headervalidation.Count > 0)
                            {
                                foreach (var error in _Headervalidation)
                                {
                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                        ValidationErrors.Add(error);
                                }
                            }
                        }
                        else
                        {
                            ValidationErrors.Add(new ErrorModel
                            {
                                errMsg = "InvoiceTypeCode is mandatory.",
                                PropertyName = "InvoiceTypeCode",
                                PropertyValue = null
                            });
                        }
                        _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.DocumentCurrencyCode, nameof(Invoice.DocumentCurrencyCode), 3);
                        if (!string.IsNullOrWhiteSpace(Invoice.DocumentCurrencyCode))
                        {
                            _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.DocumentCurrencyCode, nameof(Invoice.DocumentCurrencyCode)));
                            _Headervalidation.Add(new ValidationExtension().ValidateCurrencyType(Invoice.DocumentCurrencyCode, nameof(Invoice.DocumentCurrencyCode)));
                        }
                        if (_Headervalidation.Count > 0)
                        {
                            foreach (var error in _Headervalidation)
                            {
                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                    ValidationErrors.Add(error);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(Invoice.TaxCurrencyCode))
                        {
                            _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.TaxCurrencyCode, nameof(Invoice.TaxCurrencyCode), 3);
                            _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.TaxCurrencyCode, nameof(Invoice.TaxCurrencyCode)));
                            _Headervalidation.Add(new ValidationExtension().ValidateCurrencyType(Invoice.TaxCurrencyCode, nameof(Invoice.TaxCurrencyCode)));
                            if (_Headervalidation.Count > 0)
                            {
                                foreach (var error in _Headervalidation)
                                {
                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                        ValidationErrors.Add(error);
                                }
                            }
                        }
                        #endregion
                        #region InvoicePeriod/
                        List<ErrorModel> _InvoicePeriodErrors = new List<ErrorModel>();
                        if (Invoice.InvoicePeriod != null)
                        {
                            if (!string.IsNullOrWhiteSpace(Invoice.InvoicePeriod.StartDate))
                            {
                                _InvoicePeriodErrors = new ValidationExtension().ValidatePropertySize(Invoice.InvoicePeriod.StartDate, nameof(Invoice.InvoicePeriod) + "/BillingPeriodStartDate", 10);
                                _InvoicePeriodErrors.Add(new ValidationExtension().ValidateDateProperty(Invoice.InvoicePeriod.StartDate, nameof(Invoice.InvoicePeriod) + "/BillingPeriodStartDate"));
                                if (_InvoicePeriodErrors.Count > 0)
                                {
                                    foreach (var error in _InvoicePeriodErrors)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.InvoicePeriod.EndDate))
                            {
                                _InvoicePeriodErrors = new ValidationExtension().ValidatePropertySize(Invoice.InvoicePeriod.EndDate, nameof(Invoice.InvoicePeriod) + "/BillingPeriodEndDate", 10);
                                _InvoicePeriodErrors.Add(new ValidationExtension().ValidateDateProperty(Invoice.InvoicePeriod.EndDate, nameof(Invoice.InvoicePeriod) + "/BillingPeriodEndDate"));
                                if (_InvoicePeriodErrors.Count > 0)
                                {
                                    foreach (var error in _InvoicePeriodErrors)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.InvoicePeriod.Description))
                            {
                                _InvoicePeriodErrors = new ValidationExtension().ValidatePropertySize(Invoice.InvoicePeriod.Description, nameof(Invoice.InvoicePeriod) + "/Description", 50);
                                if (_InvoicePeriodErrors.Count > 0)
                                {
                                    foreach (var error in _InvoicePeriodErrors)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region InvoiceLineItem/
                        var _Invoiceline = new ValidationExtension().GetMultiElements<InvoicelineMulti>(PathToXml, "InvoiceLine", true, ref errorMsg);
                        if (_Invoiceline.Count > 0)
                        {
                            foreach (var invLines in _Invoiceline)
                            {
                                string invId = string.Empty;
                                List<ErrorModel> _InvoiceLineErrors = new List<ErrorModel>();
                                ErrorModel _InvoiceLinePriceErrors = new ErrorModel();
                                if (!string.IsNullOrWhiteSpace(invLines.Invoiceline.ID))
                                {
                                    _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(invLines.Invoiceline.ID, nameof(invLines.Invoiceline) + invId + " " + invLines.Invoiceline.ID + "/" + nameof(invLines.Invoiceline.ID), 26);
                                    _InvoiceLineErrors.Add(new ValidationExtension().ValidatePropertyNormalization(invLines.Invoiceline.ID, nameof(invLines.Invoiceline) + invId + " " + invLines.Invoiceline.ID + "/" + nameof(invLines.Invoiceline.ID)));
                                    if (_InvoiceLineErrors.Count > 0)
                                    {
                                        foreach (var error in _InvoiceLineErrors)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                    invId = invLines.Invoiceline.ID;
                                }
                                if (!string.IsNullOrWhiteSpace(invLines.Invoiceline.InvoicedQuantity.text))
                                {
                                    ErrorModel _QuantityErrors = new ValidationExtension().ValidateDecimalProperty(invLines.Invoiceline.InvoicedQuantity.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.InvoicedQuantity));//decimal type
                                    if (!string.IsNullOrWhiteSpace(_QuantityErrors.errMsg))
                                    {
                                        ValidationErrors.Add(_QuantityErrors);
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(invLines.Invoiceline.InvoicedQuantity.unitCode))
                                {
                                    _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(invLines.Invoiceline.InvoicedQuantity.unitCode, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.InvoicedQuantity.unitCode), 3);
                                    _InvoiceLineErrors.Add(new ValidationExtension().ValidatePropertyNormalization(invLines.Invoiceline.InvoicedQuantity.unitCode, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.InvoicedQuantity.unitCode)));
                                    _InvoiceLineErrors.Add(new ValidationExtension().ValidateUnitType(invLines.Invoiceline.InvoicedQuantity.unitCode, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.InvoicedQuantity.unitCode)));
                                    if (_InvoiceLineErrors.Count > 0)
                                    {
                                        foreach (var error in _InvoiceLineErrors)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                }
                                ErrorModel _LineExtensionAmountErrors = new ValidationExtension().ValidateDecimalProperty(invLines.Invoiceline.LineExtensionAmount.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.LineExtensionAmount));//decimal type
                                if (!string.IsNullOrWhiteSpace(_LineExtensionAmountErrors.errMsg))
                                {
                                    ValidationErrors.Add(_LineExtensionAmountErrors);
                                }
                                #region AllowanceCharges/
                                var _InvoicelineAllowanceCharge = new ValidationExtension().GetMultiElements<AllowancechargeMulti>(PathToXml, "AllowanceCharge", "InvoiceLine", true, ref errorMsg);
                                if (_InvoicelineAllowanceCharge.Count > 0)
                                {
                                    foreach (var AllowanceCharge in _InvoicelineAllowanceCharge)
                                    {
                                        _InvoiceLineErrors = new List<ErrorModel>();
                                        if (!string.IsNullOrWhiteSpace(AllowanceCharge.Allowancecharge.ChargeIndicator))
                                        {
                                            ErrorModel _AllowanceChargesErrors = new ValidationExtension().ValidateBooleanProperty(AllowanceCharge.Allowancecharge.ChargeIndicator, nameof(invLines.Invoiceline) + invId + "/" + nameof(AllowanceCharge.Allowancecharge) + "/" + nameof(AllowanceCharge.Allowancecharge.ChargeIndicator));
                                            if (!string.IsNullOrWhiteSpace(_AllowanceChargesErrors.errMsg))
                                            {
                                                ValidationErrors.Add(_AllowanceChargesErrors);
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(AllowanceCharge.Allowancecharge.MultiplierFactorNumeric))
                                        {
                                            ErrorModel _AllowanceChargesErrors = new ValidationExtension().ValidateDecimalProperty(AllowanceCharge.Allowancecharge.MultiplierFactorNumeric, nameof(invLines.Invoiceline) + invId + "/" + nameof(AllowanceCharge.Allowancecharge) + "/" + nameof(AllowanceCharge.Allowancecharge.MultiplierFactorNumeric));
                                            if (!string.IsNullOrWhiteSpace(_AllowanceChargesErrors.errMsg))
                                            {
                                                ValidationErrors.Add(_AllowanceChargesErrors);
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(AllowanceCharge.Allowancecharge.AllowanceChargeReason))
                                        {
                                            _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(AllowanceCharge.Allowancecharge.AllowanceChargeReason, nameof(invLines.Invoiceline) + invId + "/" + nameof(AllowanceCharge.Allowancecharge) + "/" + nameof(AllowanceCharge.Allowancecharge.AllowanceChargeReason), 300);
                                            if (_InvoiceLineErrors.Count > 0)
                                            {
                                                foreach (var error in _InvoiceLineErrors)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                        ValidationErrors.Add(error);
                                                }
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(AllowanceCharge.Allowancecharge.Amount.text))
                                        {
                                            ErrorModel _AllowanceChargesErrors = new ValidationExtension().ValidateDecimalProperty(AllowanceCharge.Allowancecharge.Amount.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(AllowanceCharge.Allowancecharge) + "/" + nameof(AllowanceCharge.Allowancecharge.Amount));
                                            if (!string.IsNullOrWhiteSpace(_AllowanceChargesErrors.errMsg))
                                            {
                                                ValidationErrors.Add(_AllowanceChargesErrors);
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region TaxTotal/
                                if (invLines.Invoiceline.TaxTotal != null)
                                {
                                    var _ItemTaxSubtotalMulti = new ValidationExtension().GetMultiElements<TaxsubtotalMulti>(PathToXml, "TaxSubtotal", "TaxTotal", true, ref errorMsg);
                                    ErrorModel _TaxTotalAmountErrors = new ValidationExtension().ValidateDecimalProperty(invLines.Invoiceline.TaxTotal.TaxAmount.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(invLines.Invoiceline.TaxTotal.TaxAmount));//decimal type
                                    if (!string.IsNullOrWhiteSpace(_TaxTotalAmountErrors.errMsg))
                                    {
                                        ValidationErrors.Add(_TaxTotalAmountErrors);
                                    }
                                    if (_ItemTaxSubtotalMulti.Count > 0)
                                    {
                                        foreach (var _TaxSubtotal in _ItemTaxSubtotalMulti)
                                        {
                                            _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(_TaxSubtotal.Taxsubtotal.TaxCategory.ID, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory) + "ID", 2);
                                            if (!string.IsNullOrWhiteSpace(_TaxSubtotal.Taxsubtotal.TaxCategory.ID))
                                            {
                                                _InvoiceLineErrors.Add(new ValidationExtension().ValidateTaxType(_TaxSubtotal.Taxsubtotal.TaxCategory.ID, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory) + "ID"));
                                                _InvoiceLineErrors.Add(new ValidationExtension().ValidatePropertyNormalization(_TaxSubtotal.Taxsubtotal.TaxCategory.ID, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory) + "ID"));
                                            }
                                            if (_InvoiceLineErrors.Count > 0)
                                            {
                                                foreach (var error in _InvoiceLineErrors)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                        ValidationErrors.Add(error);
                                                }
                                            }
                                            _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme.ID.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme) + "ID", 3);
                                            if (!string.IsNullOrWhiteSpace(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme.ID.text))
                                            {
                                                _InvoiceLineErrors.Add(new ValidationExtension().ValidatePropertyNormalization(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme.ID.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme) + "ID"));
                                            }
                                            if (_InvoiceLineErrors.Count > 0)
                                            {
                                                foreach (var error in _InvoiceLineErrors)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                        ValidationErrors.Add(error);
                                                }
                                            }
                                            if (_TaxSubtotal.Taxsubtotal.perUnitAmount != null)
                                            {
                                                ErrorModel _TaxRatePriceErrors = new ValidationExtension().ValidateDecimalProperty(_TaxSubtotal.Taxsubtotal.perUnitAmount.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.perUnitAmount));//decimal type
                                                if (!string.IsNullOrWhiteSpace(_TaxRatePriceErrors.errMsg))
                                                {
                                                    ValidationErrors.Add(_TaxRatePriceErrors);
                                                }
                                            }
                                            if (_TaxSubtotal.Taxsubtotal.Baseunitmeasure != null)
                                            {
                                                ErrorModel _TaxRateunitsErrors = new ValidationExtension().ValidateUnitType(_TaxSubtotal.Taxsubtotal.Baseunitmeasure.unitCode, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.Baseunitmeasure));//decimal type
                                                if (!string.IsNullOrWhiteSpace(_InvoiceLinePriceErrors.errMsg))
                                                {
                                                    ValidationErrors.Add(_InvoiceLinePriceErrors);
                                                }
                                            }
                                            if (!string.IsNullOrWhiteSpace(_TaxSubtotal.Taxsubtotal.percent))
                                            {
                                                ErrorModel _TaxRatePriceErrors = new ValidationExtension().ValidateDecimalProperty(_TaxSubtotal.Taxsubtotal.percent, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.percent));//decimal type
                                                if (!string.IsNullOrWhiteSpace(_TaxRatePriceErrors.errMsg))
                                                {
                                                    ValidationErrors.Add(_TaxRatePriceErrors);
                                                }
                                            }
                                            if (_TaxSubtotal.Taxsubtotal.TaxCategory.ID == "E" && _TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme.ID.schemeAgencyID == "06")
                                            {
                                                ErrorModel _TaxRatePriceErrors = new ErrorModel();
                                                _TaxRatePriceErrors = new ValidationExtension().ValidateDecimalProperty(_TaxSubtotal.Taxsubtotal.TaxableAmount.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxableAmount));//decimal type
                                                if (!string.IsNullOrWhiteSpace(_TaxRatePriceErrors.errMsg))
                                                {
                                                    ValidationErrors.Add(_TaxRatePriceErrors);
                                                }
                                                _TaxRatePriceErrors = new ValidationExtension().ValidateCurrencyType(_TaxSubtotal.Taxsubtotal.TaxableAmount.currencyID, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxableAmount.currencyID));//decimal type
                                                if (!string.IsNullOrWhiteSpace(_TaxRatePriceErrors.errMsg))
                                                {
                                                    ValidationErrors.Add(_TaxRatePriceErrors);
                                                }
                                                _TaxRatePriceErrors = new ValidationExtension().ValidateDecimalProperty(_TaxSubtotal.Taxsubtotal.TaxAmount.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxAmount));//decimal type
                                                if (!string.IsNullOrWhiteSpace(_TaxRatePriceErrors.errMsg))
                                                {
                                                    ValidationErrors.Add(_TaxRatePriceErrors);
                                                }
                                                _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxExemptionReason, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxExemptionReason), 300);
                                                if (!string.IsNullOrWhiteSpace(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxExemptionReason))
                                                {
                                                    _InvoiceLineErrors.Add(new ValidationExtension().ValidateSpecialStrings(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxExemptionReason, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxExemptionReason), "-"));
                                                }
                                                if (_InvoiceLineErrors.Count > 0)
                                                {
                                                    foreach (var error in _InvoiceLineErrors)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion
                                _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(invLines.Invoiceline.Item.Description, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.Item) + "/" + nameof(invLines.Invoiceline.Item.Description), 300);
                                if (_InvoiceLineErrors.Count > 0)
                                {
                                    foreach (var error in _InvoiceLineErrors)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                                if (invLines.Invoiceline.Item.OriginCountry != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(invLines.Invoiceline.Item.OriginCountry.IdentificationCode))
                                    {
                                        _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(invLines.Invoiceline.Item.OriginCountry.IdentificationCode, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.Item.OriginCountry), 3);
                                        _InvoiceLineErrors.Add(new ValidationExtension().ValidatePropertyNormalization(invLines.Invoiceline.Item.OriginCountry.IdentificationCode, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.Item.OriginCountry)));
                                        _InvoiceLineErrors.Add(new ValidationExtension().ValidateCountry(invLines.Invoiceline.Item.OriginCountry.IdentificationCode, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.Item.OriginCountry)));
                                        if (_InvoiceLineErrors.Count > 0)
                                        {

                                            foreach (var error in _InvoiceLineErrors)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                }
                                var _CommodityClassification = new ValidationExtension().GetClassificationsWithId(jsonText, ref errorMsg);
                                if (_CommodityClassification.Count > 0)
                                {
                                    foreach (var _Commodity in _CommodityClassification)
                                    {
                                        if (invId == _Commodity.ID)
                                        {
                                            var _Classification = _Commodity.CommodityClassification;
                                            foreach (var Classification in _Classification)
                                            {
                                                if (Classification.listID.ToUpper() == "CLASS")
                                                {
                                                    _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(Classification.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(Classification) + " " + Classification.listID, 3);
                                                    if (!string.IsNullOrWhiteSpace(Classification.text))
                                                    {
                                                        _InvoiceLineErrors.Add(new ValidationExtension().ValidatePropertyNormalization(Classification.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(Classification) + " " + Classification.listID));
                                                    }
                                                    if (!string.IsNullOrWhiteSpace(Classification.text))
                                                    {
                                                        ErrorModel _InvoiceLineclsErrors = new ValidationExtension().ValidateClassification(Classification.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(Classification) + " " + Classification.listID);
                                                        if (!string.IsNullOrWhiteSpace(_InvoiceLineclsErrors.errMsg))
                                                        {
                                                            ValidationErrors.Add(_InvoiceLineclsErrors);
                                                        }
                                                    }
                                                    if (_InvoiceLineErrors.Count > 0)
                                                    {

                                                        foreach (var error in _InvoiceLineErrors)
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                                ValidationErrors.Add(error);
                                                        }
                                                    }
                                                }
                                                else if (Classification.listID.ToUpper() == "PTC")
                                                {
                                                    _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(Classification.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(Classification) + " " + Classification.listID, 12);
                                                    if (!string.IsNullOrWhiteSpace(Classification.text))
                                                    {
                                                        _InvoiceLineErrors.Add(new ValidationExtension().ValidatePropertyNormalization(Classification.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(Classification) + " " + Classification.listID));
                                                    }
                                                    if (!string.IsNullOrWhiteSpace(Classification.text))
                                                    {
                                                        _InvoiceLineErrors.Add(new ValidationExtension().ValidateSpecialStrings(Classification.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(Classification) + " " + Classification.listID, "."));
                                                    }
                                                    foreach (var error in _InvoiceLineErrors)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                                else if (string.IsNullOrWhiteSpace(Classification.text))
                                                {
                                                    ValidationErrors.Add(new ErrorModel
                                                    {
                                                        errMsg = "ItemClassificationCode Cannot be empty.",
                                                        PropertyName = "ItemClassificationCode",
                                                        PropertyValue = ""
                                                    });
                                                }
                                                else
                                                {
                                                    ValidationErrors.Add(new ErrorModel
                                                    {

                                                        errMsg = "ItemClassificationCode is mandatory.",
                                                        PropertyName = "ItemClassificationCode",
                                                        PropertyValue = ""
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationErrors.Add(new ErrorModel
                                    {

                                        errMsg = "ItemClassificationCode error " + errorMsg,
                                        PropertyName = "ItemClassificationCode",
                                        PropertyValue = ""
                                    });
                                }
                                _InvoiceLinePriceErrors = new ValidationExtension().ValidateDecimalProperty(invLines.Invoiceline.Price.PriceAmount.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.Item.Description));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceLinePriceErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceLinePriceErrors);
                                }
                                ErrorModel _ItemSubtotalErrors = new ValidationExtension().ValidateDecimalProperty(invLines.Invoiceline.ItemPriceExtension.Amount.text, nameof(invLines.Invoiceline) + invId + "/" + nameof(invLines.Invoiceline.ItemPriceExtension));//decimal type
                                if (!string.IsNullOrWhiteSpace(_ItemSubtotalErrors.errMsg))
                                {
                                    ValidationErrors.Add(_ItemSubtotalErrors);
                                }
                            }
                        }
                        else
                        {
                            ValidationErrors.Add(new ErrorModel
                            {
                                errMsg = "InvoiceLine is mandatory " + errorMsg,
                                PropertyName = "InvoiceLine",
                                PropertyValue = null
                            });
                        }
                        #endregion
                        #region Optinal/
                        if (Invoice.PaymentMeans != null)
                        {
                            if (!string.IsNullOrWhiteSpace(Invoice.PaymentMeans.PaymentMeansCode))
                            {
                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.PaymentMeans.PaymentMeansCode, nameof(Invoice.PaymentMeans) + "/" + nameof(Invoice.PaymentMeans.PaymentMeansCode), 2);
                                _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.PaymentMeans.PaymentMeansCode, nameof(Invoice.PaymentMeans) + "/" + nameof(Invoice.PaymentMeans.PaymentMeansCode)));
                                _Headervalidation.Add(new ValidationExtension().ValidatePaymentMethods(Invoice.PaymentMeans.PaymentMeansCode, nameof(Invoice.PaymentMeans) + "/" + nameof(Invoice.PaymentMeans.PaymentMeansCode)));
                            }
                            if (_Headervalidation.Count > 0)
                            {
                                foreach (var error in _Headervalidation)
                                {
                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                        ValidationErrors.Add(error);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.PaymentMeans.PaymentMeansCode))
                            {
                                if (Invoice.PaymentMeans.PayeeFinancialAccount != null)
                                {
                                    _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.PaymentMeans.PayeeFinancialAccount.ID, nameof(Invoice.PaymentMeans) + "/" + nameof(Invoice.PaymentMeans.PayeeFinancialAccount), 150);
                                    _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.PaymentMeans.PayeeFinancialAccount.ID, nameof(Invoice.PaymentMeans) + "/" + nameof(Invoice.PaymentMeans.PayeeFinancialAccount)));
                                }
                            }
                            if (_Headervalidation.Count > 0)
                            {
                                foreach (var error in _Headervalidation)
                                {
                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                        ValidationErrors.Add(error);
                                }
                            }
                        }
                        if (Invoice.PaymentTerms != null)
                        {
                            if (!string.IsNullOrWhiteSpace(Invoice.PaymentTerms.Note))
                            {
                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.PaymentTerms.Note, nameof(Invoice.PaymentTerms), 300);
                                if (_Headervalidation.Count > 0)
                                {
                                    foreach (var error in _Headervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                            }
                        }
                        if (Invoice.PrepaidPayment != null)
                        {
                            if (!string.IsNullOrWhiteSpace(Invoice.PrepaidPayment.ID))
                            {
                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.PrepaidPayment.ID, nameof(Invoice.PrepaidPayment) + "/" + nameof(Invoice.PrepaidPayment.ID), 150);
                                if (!string.IsNullOrWhiteSpace(Invoice.PrepaidPayment.ID))
                                {
                                    _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.PrepaidPayment.ID, nameof(Invoice.PrepaidPayment) + "/" + nameof(Invoice.PrepaidPayment.ID)));
                                }
                                if (_Headervalidation.Count > 0)
                                {
                                    foreach (var error in _Headervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.PrepaidPayment.PaidTime))
                            {
                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.PrepaidPayment.PaidTime, nameof(Invoice.PrepaidPayment) + "/" + nameof(Invoice.PrepaidPayment.PaidTime), 9);
                                if (!string.IsNullOrWhiteSpace(Invoice.PrepaidPayment.PaidTime))
                                {
                                    _Headervalidation.Add(new ValidationExtension().ValidateTimeProperty(Invoice.PrepaidPayment.PaidTime, nameof(Invoice.PrepaidPayment) + "/" + nameof(Invoice.PrepaidPayment.PaidTime)));
                                }
                                if (_Headervalidation.Count > 0)
                                {
                                    foreach (var error in _Headervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.PrepaidPayment.PaidDate))
                            {
                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.PrepaidPayment.PaidDate, nameof(Invoice.PrepaidPayment) + "/" + nameof(Invoice.PrepaidPayment.PaidDate), 10);
                                if (!string.IsNullOrWhiteSpace(Invoice.PrepaidPayment.PaidDate))
                                {
                                    _Headervalidation.Add(new ValidationExtension().ValidateDateProperty(Invoice.PrepaidPayment.PaidDate, nameof(Invoice.PrepaidPayment) + "/" + nameof(Invoice.PrepaidPayment.PaidDate)));
                                }
                                if (_Headervalidation.Count > 0)
                                {
                                    foreach (var error in _Headervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.PrepaidPayment.PaidAmount.text))
                            {
                                ErrorModel _PrepaidPaymentErrors = new ValidationExtension().ValidateDecimalProperty(Invoice.PrepaidPayment.PaidAmount.text, nameof(Invoice.PrepaidPayment) + "/" + nameof(Invoice.PrepaidPayment.PaidAmount));
                                if (!string.IsNullOrWhiteSpace(_PrepaidPaymentErrors.errMsg))
                                {
                                    ValidationErrors.Add(_PrepaidPaymentErrors);
                                }
                                _PrepaidPaymentErrors = new ValidationExtension().ValidateCurrencyType(Invoice.PrepaidPayment.PaidAmount.currencyID, nameof(Invoice.PrepaidPayment) + "/" + nameof(Invoice.PrepaidPayment.PaidAmount.currencyID));
                                if (!string.IsNullOrWhiteSpace(_PrepaidPaymentErrors.errMsg))
                                {
                                    ValidationErrors.Add(_PrepaidPaymentErrors);
                                }
                            }
                        }
                        var _BillingReference = new ValidationExtension().GetMultiElements<BillingReferenceMulti>(PathToXml, "BillingReference", true, ref errorMsg);
                        if (_BillingReference != null)
                        {
                            if (_BillingReference.Count > 0)
                            {
                                foreach (var BillingReference in _BillingReference)
                                {
                                    if (BillingReference.Billingreference != null)
                                    {
                                        if (BillingReference.Billingreference.AdditionalDocumentReference != null)
                                        {
                                            if (!string.IsNullOrWhiteSpace(BillingReference.Billingreference.AdditionalDocumentReference.ID))
                                            {
                                                _Headervalidation = new ValidationExtension().ValidatePropertySize(BillingReference.Billingreference.AdditionalDocumentReference.ID, nameof(BillingReference.Billingreference) + "/" + "ID", 150);
                                                if (!string.IsNullOrWhiteSpace(BillingReference.Billingreference.AdditionalDocumentReference.ID))
                                                {
                                                    _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(BillingReference.Billingreference.AdditionalDocumentReference.ID, nameof(BillingReference.Billingreference) + "/" + "ID"));
                                                }
                                                if (_Headervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Headervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //if(BillingReference.Billingreference.InvoiceDocumentReference != null)
                                    //{
                                    //    if (!string.IsNullOrWhiteSpace(BillingReference.Billingreference.AdditionalDocumentReference.ID))
                                    //    {
                                    //        _Headervalidation = new ValidationExtension().ValidatePropertySize(BillingReference.Billingreference.AdditionalDocumentReference.ID, nameof(BillingReference.Billingreference) + ">BillingReference>Id", 150);
                                    //        if (!string.IsNullOrWhiteSpace(BillingReference.Billingreference.AdditionalDocumentReference.ID))
                                    //        {
                                    //            _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(BillingReference.Billingreference.AdditionalDocumentReference.ID, nameof(BillingReference.Billingreference) + ">BillingReference>Id"));
                                    //        }
                                    //        if (_Headervalidation.Count > 0)
                                    //        {
                                    //            foreach (var error in _Headervalidation)
                                    //            {
                                    //                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                    //                    ValidationErrors.Add(error);
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                }
                            }
                        }
                        #endregion
                        #region LegalMonetaryTotal/
                        if (Invoice.LegalMonetaryTotal != null)
                        {
                            ErrorModel _InvoiceLegalErrors = new ValidationExtension().ValidateDecimalProperty(Invoice.LegalMonetaryTotal.TaxExclusiveAmount.text, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.TaxExclusiveAmount));//decimal type
                            if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                            {
                                ValidationErrors.Add(_InvoiceLegalErrors);
                            }
                            _InvoiceLegalErrors = new ValidationExtension().ValidateCurrencyType(Invoice.LegalMonetaryTotal.TaxExclusiveAmount.currencyID, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.TaxExclusiveAmount) + "/" + nameof(Invoice.LegalMonetaryTotal.TaxExclusiveAmount.currencyID));//decimal type
                            if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                            {
                                ValidationErrors.Add(_InvoiceLegalErrors);
                            }
                            _InvoiceLegalErrors = new ValidationExtension().ValidateDecimalProperty(Invoice.LegalMonetaryTotal.TaxInclusiveAmount.text, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.TaxInclusiveAmount));//decimal type
                            if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                            {
                                ValidationErrors.Add(_InvoiceLegalErrors);
                            }
                            _InvoiceLegalErrors = new ValidationExtension().ValidateCurrencyType(Invoice.LegalMonetaryTotal.TaxInclusiveAmount.currencyID, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.TaxInclusiveAmount) + "/" + nameof(Invoice.LegalMonetaryTotal.TaxInclusiveAmount.currencyID));//decimal type
                            if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                            {
                                ValidationErrors.Add(_InvoiceLegalErrors);
                            }
                            _InvoiceLegalErrors = new ValidationExtension().ValidateDecimalProperty(Invoice.LegalMonetaryTotal.PayableAmount.text, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.PayableAmount));//decimal type
                            if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                            {
                                ValidationErrors.Add(_InvoiceLegalErrors);
                            }
                            _InvoiceLegalErrors = new ValidationExtension().ValidateCurrencyType(Invoice.LegalMonetaryTotal.PayableAmount.currencyID, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.PayableAmount) + "/" + nameof(Invoice.LegalMonetaryTotal.PayableAmount.currencyID));//decimal type
                            if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                            {
                                ValidationErrors.Add(_InvoiceLegalErrors);
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.LegalMonetaryTotal.LineExtensionAmount.text))
                            {
                                _InvoiceLegalErrors = new ValidationExtension().ValidateDecimalProperty(Invoice.LegalMonetaryTotal.LineExtensionAmount.text, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.LineExtensionAmount));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceLegalErrors);
                                }
                                _InvoiceLegalErrors = new ValidationExtension().ValidateCurrencyType(Invoice.LegalMonetaryTotal.LineExtensionAmount.currencyID, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.LineExtensionAmount) + "/" + nameof(Invoice.LegalMonetaryTotal.LineExtensionAmount.currencyID));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceLegalErrors);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.LegalMonetaryTotal.AllowanceTotalAmount.text))
                            {
                                _InvoiceLegalErrors = new ValidationExtension().ValidateDecimalProperty(Invoice.LegalMonetaryTotal.AllowanceTotalAmount.text, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.AllowanceTotalAmount));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceLegalErrors);
                                }
                                _InvoiceLegalErrors = new ValidationExtension().ValidateCurrencyType(Invoice.LegalMonetaryTotal.AllowanceTotalAmount.currencyID, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.AllowanceTotalAmount) + "/" + nameof(Invoice.LegalMonetaryTotal.AllowanceTotalAmount.currencyID));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceLegalErrors);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.LegalMonetaryTotal.ChargeTotalAmount.text))
                            {
                                _InvoiceLegalErrors = new ValidationExtension().ValidateDecimalProperty(Invoice.LegalMonetaryTotal.ChargeTotalAmount.text, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.ChargeTotalAmount));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceLegalErrors);
                                }
                                _InvoiceLegalErrors = new ValidationExtension().ValidateCurrencyType(Invoice.LegalMonetaryTotal.ChargeTotalAmount.currencyID, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.ChargeTotalAmount.currencyID));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceLegalErrors);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.LegalMonetaryTotal.PayableRoundingAmount.text))
                            {
                                _InvoiceLegalErrors = new ValidationExtension().ValidateDecimalProperty(Invoice.LegalMonetaryTotal.PayableRoundingAmount.text, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.PayableRoundingAmount));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceLegalErrors);
                                }
                                _InvoiceLegalErrors = new ValidationExtension().ValidateCurrencyType(Invoice.LegalMonetaryTotal.PayableRoundingAmount.currencyID, nameof(Invoice.LegalMonetaryTotal) + "/" + nameof(Invoice.LegalMonetaryTotal.PayableRoundingAmount.text));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceLegalErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceLegalErrors);
                                }
                            }
                        }
                        else
                        {
                            ValidationErrors.Add(new ErrorModel
                            {

                                errMsg = "LegalMonetaryTotal is mandatory.",
                                PropertyName = "LegalMonetaryTotal",
                                PropertyValue = "LegalMonetaryTotal"
                            });
                        }
                        #endregion
                        #region InvoiceTaxToal/
                        List<ErrorModel> _InvoiceTaxErrors = new List<ErrorModel>();
                        ErrorModel _InvoiceTaxDErrors = new ErrorModel();
                        if (Invoice.TaxTotal != null)
                        {
                            if (Invoice.TaxTotal.TaxAmount != null)
                            {
                                _InvoiceTaxDErrors = new ValidationExtension().ValidateDecimalProperty(Invoice.TaxTotal.TaxAmount.text, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(Invoice.TaxTotal.TaxAmount));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceTaxDErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceTaxDErrors);
                                }
                                _InvoiceTaxDErrors = new ValidationExtension().ValidateCurrencyType(Invoice.TaxTotal.TaxAmount.currencyID, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(Invoice.TaxTotal.TaxAmount.currencyID));//decimal type
                                if (!string.IsNullOrWhiteSpace(_InvoiceTaxDErrors.errMsg))
                                {
                                    ValidationErrors.Add(_InvoiceTaxDErrors);
                                }
                            }
                            else
                            {
                                ValidationErrors.Add(new ErrorModel
                                {
                                    errMsg = nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(Invoice.TaxTotal.TaxAmount) + " is mandatory.",
                                    PropertyName = nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(Invoice.TaxTotal.TaxAmount),
                                    PropertyValue = null
                                });
                            }

                            var _TaxSubtotalMulti = new ValidationExtension().GetMultiElements<TaxsubtotalMulti>(PathToXml, "TaxSubtotal", "TaxTotal", true, ref errorMsg);
                            if (_TaxSubtotalMulti.Count > 0)
                            {
                                foreach (var _TaxSubtotal in _TaxSubtotalMulti)
                                {
                                    _InvoiceTaxErrors = new ValidationExtension().ValidatePropertySize(_TaxSubtotal.Taxsubtotal.TaxCategory.ID, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory) + "ID", 2);
                                    if (!string.IsNullOrWhiteSpace(_TaxSubtotal.Taxsubtotal.TaxCategory.ID))
                                    {
                                        _InvoiceTaxErrors.Add(new ValidationExtension().ValidatePropertyNormalization(_TaxSubtotal.Taxsubtotal.TaxCategory.ID, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory) + "ID"));
                                        _InvoiceTaxErrors.Add(new ValidationExtension().ValidateTaxType(_TaxSubtotal.Taxsubtotal.TaxCategory.ID, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory) + "ID"));
                                    }
                                    if (_InvoiceTaxErrors.Count > 0)
                                    {
                                        foreach (var error in _InvoiceTaxErrors)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                    _InvoiceTaxErrors = new ValidationExtension().ValidatePropertySize(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme.ID.text, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme), 3);
                                    if (!string.IsNullOrWhiteSpace(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme.ID.text))
                                    {
                                        _InvoiceTaxErrors.Add(new ValidationExtension().ValidatePropertyNormalization(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme.ID.text, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme)));
                                    }
                                    if (_InvoiceTaxErrors.Count > 0)
                                    {
                                        foreach (var error in _InvoiceTaxErrors)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                    if (_TaxSubtotal.Taxsubtotal.TaxCategory.ID == "E" && _TaxSubtotal.Taxsubtotal.TaxCategory.TaxScheme.ID.schemeAgencyID == "06")
                                    {
                                        _InvoiceTaxDErrors = new ValidationExtension().ValidateDecimalProperty(_TaxSubtotal.Taxsubtotal.TaxAmount.text, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxAmount));//decimal type
                                        if (!string.IsNullOrWhiteSpace(_InvoiceTaxDErrors.errMsg))
                                        {
                                            ValidationErrors.Add(_InvoiceTaxDErrors);
                                        }
                                        _InvoiceTaxDErrors = new ValidationExtension().ValidateDecimalProperty(_TaxSubtotal.Taxsubtotal.TaxableAmount.text, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxableAmount));//decimal type
                                        if (!string.IsNullOrWhiteSpace(_InvoiceTaxDErrors.errMsg))
                                        {
                                            ValidationErrors.Add(_InvoiceTaxDErrors);
                                        }
                                        _InvoiceTaxDErrors = new ValidationExtension().ValidateCurrencyType(_TaxSubtotal.Taxsubtotal.TaxableAmount.currencyID, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxableAmount));//decimal type
                                        if (!string.IsNullOrWhiteSpace(_InvoiceTaxDErrors.errMsg))
                                        {
                                            ValidationErrors.Add(_InvoiceTaxDErrors);
                                        }
                                        _InvoiceTaxErrors = new ValidationExtension().ValidatePropertySize(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxExemptionReason, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxExemptionReason), 300);
                                        _InvoiceTaxErrors.Add(new ValidationExtension().ValidateSpecialStrings(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxExemptionReason, nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal) + "/" + nameof(_TaxSubtotal.Taxsubtotal.TaxCategory.TaxExemptionReason), "-"));
                                        if (_InvoiceTaxErrors.Count > 0)
                                        {
                                            foreach (var error in _InvoiceTaxErrors)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ValidationErrors.Add(new ErrorModel
                                {
                                    errMsg = nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + "TaxSubtotal is mandatory " + errorMsg,
                                    PropertyName = nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + "TaxSubtotal",
                                    PropertyValue = null
                                });
                            }
                        }
                        else
                        {
                            ValidationErrors.Add(new ErrorModel
                            {
                                errMsg = nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/" + " is mandatory.",
                                PropertyName = nameof(Invoice) + "/" + nameof(Invoice.TaxTotal) + "/",
                                PropertyValue = null
                            });
                        }

                        #endregion
                        #region invoiceAllowanceCharges/
                        var _AllowanceCharge = new ValidationExtension().GetMultiElements<AllowancechargeMulti>(PathToXml, "AllowanceCharge", true, ref errorMsg);
                        if (_AllowanceCharge.Count > 0)
                        {
                            foreach (var AllowanceCharge in _AllowanceCharge)
                            {
                                List<ErrorModel> _InvoiceLineErrors = new List<ErrorModel>();
                                if (!string.IsNullOrWhiteSpace(AllowanceCharge.Allowancecharge.ChargeIndicator))
                                {
                                    ErrorModel _AllowanceChargesErrors = new ValidationExtension().ValidateBooleanProperty(AllowanceCharge.Allowancecharge.ChargeIndicator, nameof(Invoice) + "/" + nameof(AllowanceCharge.Allowancecharge) + "/" + nameof(AllowanceCharge.Allowancecharge.ChargeIndicator));
                                    if (!string.IsNullOrWhiteSpace(_AllowanceChargesErrors.errMsg))
                                    {
                                        ValidationErrors.Add(_AllowanceChargesErrors);
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(AllowanceCharge.Allowancecharge.MultiplierFactorNumeric))
                                {
                                    ErrorModel _AllowanceChargesErrors = new ValidationExtension().ValidateDecimalProperty(AllowanceCharge.Allowancecharge.MultiplierFactorNumeric, nameof(Invoice) + "/" + nameof(AllowanceCharge.Allowancecharge) + "/" + nameof(AllowanceCharge.Allowancecharge.MultiplierFactorNumeric));
                                    if (!string.IsNullOrWhiteSpace(_AllowanceChargesErrors.errMsg))
                                    {
                                        ValidationErrors.Add(_AllowanceChargesErrors);
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(AllowanceCharge.Allowancecharge.AllowanceChargeReason))
                                {
                                    _InvoiceLineErrors = new ValidationExtension().ValidatePropertySize(AllowanceCharge.Allowancecharge.AllowanceChargeReason, nameof(Invoice) + "/" + nameof(AllowanceCharge.Allowancecharge) + "/" + nameof(AllowanceCharge.Allowancecharge.AllowanceChargeReason), 300);
                                    if (_InvoiceLineErrors.Count > 0)
                                    {
                                        foreach (var error in _InvoiceLineErrors)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(AllowanceCharge.Allowancecharge.Amount.text))
                                {
                                    ErrorModel _AllowanceChargesErrors = new ValidationExtension().ValidateDecimalProperty(AllowanceCharge.Allowancecharge.Amount.text, nameof(Invoice) + "/" + nameof(AllowanceCharge.Allowancecharge) + "/" + nameof(AllowanceCharge.Allowancecharge.Amount));
                                    if (!string.IsNullOrWhiteSpace(_AllowanceChargesErrors.errMsg))
                                    {
                                        ValidationErrors.Add(_AllowanceChargesErrors);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region Delivery/
                        if (Invoice.Delivery != null)
                        {
                            if (Invoice.Delivery.DeliveryParty != null)
                            {
                                var _DeliveryPartyIdentification = new ValidationExtension().GetMultiElements<PartyIdentificationMulti>(PathToXml, "PartyIdentification", "Delivery", "DeliveryParty", true, ref errorMsg);
                                if (_DeliveryPartyIdentification != null)
                                {
                                    foreach (var Id in _DeliveryPartyIdentification)
                                    {
                                        if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.schemeID))
                                        {
                                            if (Id.PartyIdentification.ID.schemeID == "TIN")
                                            {
                                                _Headervalidation = new ValidationExtension().ValidatePropertyBySizeRange(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID, 9, 14);
                                                if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                                {
                                                    _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID));
                                                    _Headervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                                }
                                                if (_Headervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Headervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }

                                            }
                                            else if (Id.PartyIdentification.ID.schemeID == "NRIC" || Id.PartyIdentification.ID.schemeID == "PASSPORT" || Id.PartyIdentification.ID.schemeID == "ARMY")
                                            {
                                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID, 12);
                                                if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                                {
                                                    _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID));
                                                    _Headervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                                }
                                                if (_Headervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Headervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }

                                            }
                                            else if (Id.PartyIdentification.ID.schemeID == "BRN")
                                            {
                                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID, 20);
                                                if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                                {
                                                    _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID));
                                                    _Headervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                                }
                                                if (_Headervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Headervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID, 35);
                                                if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                                {
                                                    _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID));
                                                    _Headervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                                }
                                                if (_Headervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Headervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (Invoice.Delivery.DeliveryParty.PartyLegalEntity != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(Invoice.Delivery.DeliveryParty.PartyLegalEntity.RegistrationName))
                                    {
                                        _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.Delivery.DeliveryParty.PartyLegalEntity.RegistrationName, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + nameof(Invoice.Delivery.DeliveryParty.PartyLegalEntity) + "/" + nameof(Invoice.Delivery.DeliveryParty.PartyLegalEntity.RegistrationName), 300);
                                        if (_Headervalidation.Count > 0)
                                        {
                                            foreach (var error in _Headervalidation)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                }
                                if (Invoice.Delivery.DeliveryParty.PostalAddres != null)
                                {
                                    int j = 0;
                                    var _DeliveryPartyAddresslines = new ValidationExtension().GetMultiElements<AddresslinesMulti>(PathToXml, "AddressLine", "Delivery", "DeliveryParty", "PostalAddress", true, ref errorMsg);
                                    if (_DeliveryPartyAddresslines.Count > 0)
                                    {
                                        foreach (var AddressLines in _DeliveryPartyAddresslines)
                                        {
                                            _Headervalidation = new ValidationExtension().ValidatePropertySize(AddressLines.Addressline.Line, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + nameof(AddressLines.Addressline) + $" {j + 1}", 150);
                                            if (_Headervalidation.Count > 0)
                                            {
                                                foreach (var error in _Headervalidation)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                        ValidationErrors.Add(error);
                                                }
                                            }
                                            j++;
                                        }
                                    }
                                    _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.Delivery.DeliveryParty.PostalAddres.PostalZone, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres.PostalZone), 50);
                                    if (_Headervalidation.Count > 0)
                                    {
                                        foreach (var error in _Headervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                    _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.Delivery.DeliveryParty.PostalAddres.CityName, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres.CityName), 50);
                                    if (_Headervalidation.Count > 0)
                                    {
                                        foreach (var error in _Headervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                    _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.Delivery.DeliveryParty.PostalAddres.CountrySubentityCode, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres.CountrySubentityCode), 50);
                                    if (!string.IsNullOrWhiteSpace(Invoice.Delivery.DeliveryParty.PostalAddres.CountrySubentityCode))
                                    {
                                        ValidationErrors.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.Delivery.DeliveryParty.PostalAddres.CountrySubentityCode, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres.CountrySubentityCode)));
                                        ValidationErrors.Add(new ValidationExtension().ValidateStates(Invoice.Delivery.DeliveryParty.PostalAddres.CountrySubentityCode, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres.CountrySubentityCode)));
                                    }
                                    if (_Headervalidation.Count > 0)
                                    {
                                        foreach (var error in _Headervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                    _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.Delivery.DeliveryParty.PostalAddres.Country.IdentificationCode.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres.Country), 3);
                                    if (!string.IsNullOrWhiteSpace(Invoice.Delivery.DeliveryParty.PostalAddres.Country.IdentificationCode.text))
                                    {
                                        _Headervalidation.Add(new ValidationExtension().ValidateCountry(Invoice.Delivery.DeliveryParty.PostalAddres.Country.IdentificationCode.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres) + "/" + nameof(Invoice.Delivery.DeliveryParty.PostalAddres.Country)));
                                    }
                                    if (_Headervalidation.Count > 0)
                                    {
                                        foreach (var error in _Headervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                }
                            }
                            if (Invoice.Delivery.Shipment != null)
                            {
                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.Delivery.Shipment.ID, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.Shipment) + "Id", 26);
                                if (!string.IsNullOrWhiteSpace(Invoice.Delivery.Shipment.ID))
                                {
                                    _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.Delivery.Shipment.ID, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.Shipment) + "Id"));
                                }
                                if (_Headervalidation.Count > 0)
                                {
                                    foreach (var error in _Headervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                                ErrorModel _DeliveryChargeErrors = new ValidationExtension().ValidateBooleanProperty(Invoice.Delivery.Shipment.FreightAllowanceCharge.ChargeIndicator, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.Shipment) + "/" + nameof(Invoice.Delivery.Shipment.FreightAllowanceCharge) + "/" + nameof(Invoice.Delivery.Shipment.FreightAllowanceCharge.ChargeIndicator));
                                if (!string.IsNullOrWhiteSpace(_DeliveryChargeErrors.errMsg))
                                {
                                    ValidationErrors.Add(_DeliveryChargeErrors);
                                }
                                _DeliveryChargeErrors = new ValidationExtension().ValidateDecimalProperty(Invoice.Delivery.Shipment.FreightAllowanceCharge.Amount.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.Shipment) + "/" + nameof(Invoice.Delivery.Shipment.FreightAllowanceCharge) + "/" + nameof(Invoice.Delivery.Shipment.FreightAllowanceCharge.Amount));
                                if (!string.IsNullOrWhiteSpace(_DeliveryChargeErrors.errMsg))
                                {
                                    ValidationErrors.Add(_DeliveryChargeErrors);
                                }
                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.Delivery.Shipment.ID, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.Shipment) + "/" + nameof(Invoice.Delivery.Shipment.FreightAllowanceCharge) + "/" + nameof(Invoice.Delivery.Shipment.FreightAllowanceCharge.AllowanceChargeReason), 300);
                                if (_Headervalidation.Count > 0)
                                {
                                    foreach (var error in _Headervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                            }
                        }
                        #endregion
                        #region Supplier/
                        if (Invoice.AccountingSupplierParty != null)
                        {
                            if (Invoice.AccountingSupplierParty.Party != null)
                            {
                                List<ErrorModel> _Suppliervalidation = new List<ErrorModel>();
                                if (Invoice.AccountingSupplierParty.Party.IndustryClassificationCode != null)
                                {
                                    _Suppliervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode), 5);
                                    if (!string.IsNullOrWhiteSpace(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode.text))
                                    {
                                        _Suppliervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode)));
                                        _Suppliervalidation.Add(new ValidationExtension().ValidateMSICType(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode)));
                                    }
                                    if (_Suppliervalidation.Count > 0)
                                    {
                                        foreach (var error in _Suppliervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                    _Suppliervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode.name, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode) + " name", 300);
                                    if (_Suppliervalidation.Count > 0)
                                    {
                                        foreach (var error in _Suppliervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationErrors.Add(new ErrorModel
                                    {
                                        errMsg = nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode) + " cannot be empty.",
                                        PropertyName = nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.IndustryClassificationCode),
                                        PropertyValue = null
                                    });
                                }
                                var _AccountingSupplierPartyIdentification = new ValidationExtension().GetMultiElements<PartyIdentificationMulti>(PathToXml, "PartyIdentification", "AccountingSupplierParty", "Party", true, ref errorMsg);
                                if (_AccountingSupplierPartyIdentification.Count > 0)
                                {
                                    string schemeIDs = string.Join(",", _AccountingSupplierPartyIdentification.Select(x => x.PartyIdentification?.ID?.schemeID).ToList());
                                    if (schemeIDs.Length > 0)
                                    {
                                        if (!schemeIDs.Contains("TIN"))
                                        {
                                            ValidationErrors.Add(new ErrorModel
                                            {
                                                errMsg = "TIN is mandatory.",
                                                PropertyName = "AccountingSupplierParty",
                                                PropertyValue = null
                                            });
                                        }
                                        if (!(schemeIDs.Contains("NRIC") || schemeIDs.Contains("PASSPORT") || schemeIDs.Contains("ARMY") || schemeIDs.Contains("BRN")))
                                        {
                                            ValidationErrors.Add(new ErrorModel
                                            {
                                                errMsg = "schemeID,schemeIDNo is mandatory.",
                                                PropertyName = "AccountingSupplierParty",
                                                PropertyValue = null
                                            });
                                        }
                                    }
                                    foreach (var Id in _AccountingSupplierPartyIdentification)
                                    {
                                        if (Id.PartyIdentification != null)
                                        {
                                            if (Id.PartyIdentification.ID.schemeID == "TIN")
                                            {
                                                _Suppliervalidation = new ValidationExtension().ValidatePropertyBySizeRange(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, 9, 14);
                                                if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                                {
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID));
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                                }
                                                if (_Suppliervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Suppliervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                            }
                                            else if (Id.PartyIdentification.ID.schemeID == "NRIC" || Id.PartyIdentification.ID.schemeID == "PASSPORT" || Id.PartyIdentification.ID.schemeID == "ARMY")
                                            {
                                                _Suppliervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, 12);
                                                if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                                {
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID));
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                                }
                                                if (_Suppliervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Suppliervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                            }
                                            else if (Id.PartyIdentification.ID.schemeID == "BRN")
                                            {
                                                _Suppliervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, 20);
                                                if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                                {
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID));
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                                }
                                                if (_Suppliervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Suppliervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                            }
                                            else if (Id.PartyIdentification.ID.schemeID == "SST")
                                            {
                                                _Suppliervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, 35);
                                                if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                                {
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID));
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                                }
                                                if (_Suppliervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Suppliervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                            }
                                            else if (Id.PartyIdentification.ID.schemeID == "TTX")
                                            {
                                                _Suppliervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, 17);
                                                if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                                {
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID));
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, "-"));
                                                }
                                                if (_Suppliervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Suppliervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.Delivery) + "/" + nameof(Invoice.Delivery.DeliveryParty) + "/" + Id.PartyIdentification.ID.schemeID, 35);
                                                if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                                {
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID));
                                                    _Suppliervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                                }
                                                if (_Headervalidation.Count > 0)
                                                {
                                                    foreach (var error in _Headervalidation)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationErrors.Add(new ErrorModel
                                    {
                                        errMsg = "TIN,schemeID,schemeIDNo is mandatory " + errorMsg,
                                        PropertyName = "AccountingSupplierParty",
                                        PropertyValue = null
                                    });
                                }
                                if (Invoice.AccountingSupplierParty.Party.PostalAddress != null)
                                {
                                    int i = 0;
                                    var _DeliveryPartyAddresslines = new ValidationExtension().GetMultiElements<AddresslinesMulti>(PathToXml, "AddressLine", "AccountingSupplierParty", "Party", "PostalAddress", true, ref errorMsg);
                                    if (_DeliveryPartyAddresslines.Count > 0)
                                    {
                                        foreach (var AddressLines in _DeliveryPartyAddresslines)
                                        {
                                            _Headervalidation = new ValidationExtension().ValidatePropertySize(AddressLines.Addressline.Line, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress) + "/" + nameof(AddressLines.Addressline) + $" {i + 1}", 150);
                                            if (_Headervalidation.Count > 0)
                                            {
                                                foreach (var error in _Headervalidation)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                        ValidationErrors.Add(error);
                                                }
                                            }
                                            i++;
                                        }
                                    }
                                    else
                                    {
                                        ValidationErrors.Add(new ErrorModel
                                        {

                                            errMsg = "AccountingSupplierParty Address Line can't be empty " + errorMsg,
                                            PropertyName = "AccountingSupplierParty AddressLine",
                                            PropertyValue = "AccountingSupplierParty AddressLine"
                                        });
                                    }
                                    if (!string.IsNullOrWhiteSpace(Invoice.AccountingSupplierParty.Party.PostalAddress.PostalZone))
                                    {
                                        _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingSupplierParty.Party.PostalAddress.PostalZone, nameof(Invoice.AccountingSupplierParty) + "PostalZone", 50);
                                        if (_Headervalidation.Count > 0)
                                        {
                                            foreach (var error in _Headervalidation)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                    _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingSupplierParty.Party.PostalAddress.CityName, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress.CityName), 50);
                                    if (_Headervalidation.Count > 0)
                                    {
                                        foreach (var error in _Headervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                    _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingSupplierParty.Party.PostalAddress.CountrySubentityCode, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress.CountrySubentityCode), 50);
                                    if (!string.IsNullOrWhiteSpace(Invoice.AccountingSupplierParty.Party.PostalAddress.CountrySubentityCode))
                                    {
                                        _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.AccountingSupplierParty.Party.PostalAddress.CountrySubentityCode, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress.CountrySubentityCode)));
                                        _Headervalidation.Add(new ValidationExtension().ValidateStates(Invoice.AccountingSupplierParty.Party.PostalAddress.CountrySubentityCode, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress.CountrySubentityCode)));
                                    }
                                    if (_Headervalidation.Count > 0)
                                    {
                                        foreach (var error in _Headervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                    if (Invoice.AccountingSupplierParty.Party.PostalAddress.Country != null)
                                    {
                                        _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingSupplierParty.Party.PostalAddress.Country.IdentificationCode.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress.Country), 3);
                                        if (!string.IsNullOrWhiteSpace(Invoice.AccountingSupplierParty.Party.PostalAddress.Country.IdentificationCode.text))
                                        {
                                            _Headervalidation.Add(new ValidationExtension().ValidateCountry(Invoice.AccountingSupplierParty.Party.PostalAddress.Country.IdentificationCode.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress.Country)));
                                        }
                                        if (_Headervalidation.Count > 0)
                                        {
                                            foreach (var error in _Headervalidation)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ValidationErrors.Add(new ErrorModel
                                        {
                                            errMsg = nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress.Country) + " is mandatory.",
                                            PropertyName = nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PostalAddress.Country),
                                            PropertyValue = null
                                        });
                                    }
                                }
                                else
                                {
                                    ValidationErrors.Add(new ErrorModel
                                    {
                                        errMsg = "AccountingSupplierParty PostalAddres is mandatory.",
                                        PropertyName = "AccountingSupplierParty PostalAddres",
                                        PropertyValue = "AccountingSupplierParty PostalAddres"
                                    });
                                }
                                _Suppliervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingSupplierParty.Party.PartyLegalEntity.RegistrationName, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PartyLegalEntity) + "/" + nameof(Invoice.AccountingSupplierParty.Party.PartyLegalEntity.RegistrationName), 300);
                                if (_Suppliervalidation.Count > 0)
                                {
                                    foreach (var error in _Suppliervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(Invoice.AccountingCustomerParty.Party.Contact.ElectronicMail))
                                {
                                    _Suppliervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingSupplierParty.Party.Contact.ElectronicMail, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.Contact) + "/" + nameof(Invoice.AccountingSupplierParty.Party.Contact.ElectronicMail), 320);
                                    _Suppliervalidation.Add(new ValidationExtension().ValidateEmailFormat(Invoice.AccountingSupplierParty.Party.Contact.ElectronicMail, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.Contact) + "/" + nameof(Invoice.AccountingSupplierParty.Party.Contact.ElectronicMail)));
                                    if (_Suppliervalidation.Count > 0)
                                    {
                                        foreach (var error in _Suppliervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                }
                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingSupplierParty.Party.Contact.Telephone, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party) + "/" + nameof(Invoice.AccountingSupplierParty.Party.Contact) + "/" + nameof(Invoice.AccountingSupplierParty.Party.Contact.Telephone), 20);
                                if (_Headervalidation.Count > 0)
                                {
                                    foreach (var error in _Headervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                            }
                            else
                            {
                                ValidationErrors.Add(new ErrorModel
                                {
                                    errMsg = nameof(Invoice.AccountingSupplierParty.Party) + " cannot be empty.",
                                    PropertyName = nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.Party),
                                    PropertyValue = null
                                });
                            }
                            if (Invoice.AccountingSupplierParty.AdditionalAccountID != null)
                            {
                                if (!string.IsNullOrWhiteSpace(Invoice.AccountingSupplierParty.AdditionalAccountID.text))
                                {
                                    _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingSupplierParty.AdditionalAccountID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.AdditionalAccountID), 300);
                                    _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.AccountingSupplierParty.AdditionalAccountID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.AdditionalAccountID)));
                                    _Headervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Invoice.AccountingSupplierParty.AdditionalAccountID.text, nameof(Invoice.AccountingSupplierParty) + "/" + nameof(Invoice.AccountingSupplierParty.AdditionalAccountID), "-"));
                                    if (_Headervalidation.Count > 0)
                                    {
                                        foreach (var error in _Headervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ValidationErrors.Add(new ErrorModel
                            {
                                errMsg = "AccountingSupplierParty cannot be empty.",
                                PropertyName = "AccountingSupplierParty",
                                PropertyValue = null
                            });
                        }
                        #endregion
                        #region Customer/
                        if (Invoice.AccountingCustomerParty != null)
                        {
                            List<ErrorModel> _Customervalidation = new List<ErrorModel>();
                            var _AccountingCustomerPartyIdentification = new ValidationExtension().GetMultiElements<PartyIdentificationMulti>(PathToXml, "PartyIdentification", "AccountingCustomerParty", "Party", true, ref errorMsg);
                            if (_AccountingCustomerPartyIdentification.Count > 0)
                            {
                                string schemeIDs = string.Join(",", _AccountingCustomerPartyIdentification.Select(x => x.PartyIdentification?.ID?.schemeID).ToList());
                                if (schemeIDs.Length > 0)
                                {
                                    if (!schemeIDs.Contains("TIN"))
                                    {
                                        ValidationErrors.Add(new ErrorModel
                                        {
                                            errMsg = "TIN is mandatory.",
                                            PropertyName = "AccountingCustomerParty",
                                            PropertyValue = null
                                        });
                                    }
                                    if (!(schemeIDs.Contains("NRIC") || schemeIDs.Contains("PASSPORT") || schemeIDs.Contains("ARMY") || schemeIDs.Contains("BRN")))
                                    {
                                        ValidationErrors.Add(new ErrorModel
                                        {
                                            errMsg = "schemeID,schemeIDNo is mandatory.",
                                            PropertyName = "AccountingCustomerParty",
                                            PropertyValue = null
                                        });
                                    }
                                }
                                foreach (var Id in _AccountingCustomerPartyIdentification)
                                {
                                    if (Id.PartyIdentification.ID.schemeID == "TIN")
                                    {
                                        _Customervalidation = new ValidationExtension().ValidatePropertyBySizeRange(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, 9, 14);
                                        if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                        {
                                            _Customervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID));
                                            _Customervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                        }
                                        if (_Customervalidation.Count > 0)
                                        {
                                            foreach (var error in _Customervalidation)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                    else if (Id.PartyIdentification.ID.schemeID == "NRIC" || Id.PartyIdentification.ID.schemeID == "PASSPORT" || Id.PartyIdentification.ID.schemeID == "ARMY")
                                    {
                                        _Customervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, 12);
                                        if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                        {
                                            _Customervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID));
                                            _Customervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                        }
                                        if (_Customervalidation.Count > 0)
                                        {
                                            foreach (var error in _Customervalidation)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                    else if (Id.PartyIdentification.ID.schemeID == "BRN")
                                    {
                                        _Customervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, 20);
                                        if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                        {
                                            _Customervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID));
                                            _Customervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                        }
                                        if (_Customervalidation.Count > 0)
                                        {
                                            foreach (var error in _Customervalidation)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                    else if (Id.PartyIdentification.ID.schemeID == "SST")
                                    {
                                        _Customervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, 35);
                                        if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                        {
                                            _Customervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID));
                                            _Customervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, "-;"));
                                        }
                                        if (_Customervalidation.Count > 0)
                                        {
                                            foreach (var error in _Customervalidation)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                    else if (Id.PartyIdentification.ID.schemeID == "TTX")
                                    {
                                        _Customervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, 17);
                                        if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text))
                                        {
                                            _Customervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID));
                                            _Customervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, "-"));
                                        }
                                        if (_Customervalidation.Count > 0)
                                        {
                                            foreach (var error in _Customervalidation)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _Headervalidation = new ValidationExtension().ValidatePropertySize(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, 35);
                                        if (!string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.text) && !string.IsNullOrWhiteSpace(Id.PartyIdentification.ID.schemeID))
                                        {
                                            _Customervalidation.Add(new ValidationExtension().ValidateSpecialStrings(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID, ""));
                                            _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Id.PartyIdentification.ID.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Id.PartyIdentification) + "/" + Id.PartyIdentification.ID.schemeID));
                                        }
                                        if (_Headervalidation.Count > 0)
                                        {
                                            foreach (var error in _Headervalidation)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ValidationErrors.Add(new ErrorModel
                                {
                                    errMsg = "TIN,schemeID,schemeIDNo is mandatory " + errorMsg,
                                    PropertyName = "AccountingSupplierParty",
                                    PropertyValue = null
                                });
                            }
                            if (Invoice.AccountingCustomerParty.Party.PostalAddress != null)
                            {
                                int k = 0;
                                var _DeliveryPartyAddresslines = new ValidationExtension().GetMultiElements<AddresslinesMulti>(PathToXml, "AddressLine", "AccountingCustomerParty", "Party", "PostalAddress", true, ref errorMsg);
                                if (_DeliveryPartyAddresslines.Count > 0)
                                {
                                    foreach (var AddressLines in _DeliveryPartyAddresslines)
                                    {
                                        _Headervalidation = new ValidationExtension().ValidatePropertySize(AddressLines.Addressline.Line, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress) + "/" + nameof(AddressLines.Addressline) + $" {k + 1}", 150);
                                        if (_Headervalidation.Count > 0)
                                        {
                                            foreach (var error in _Headervalidation)
                                            {
                                                if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                    ValidationErrors.Add(error);
                                            }
                                        }
                                        k++;
                                    }
                                }
                                else
                                {
                                    ValidationErrors.Add(new ErrorModel
                                    {

                                        errMsg = "AccountingCustomerParty Address Line can't be empty " + errorMsg,
                                        PropertyName = "AccountingCustomerParty AddressLine",
                                        PropertyValue = "AccountingCustomerParty AddressLine"
                                    });
                                }
                                if (!string.IsNullOrWhiteSpace(Invoice.AccountingCustomerParty.Party.PostalAddress.PostalZone))
                                {
                                    _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingCustomerParty.Party.PostalAddress.PostalZone, nameof(Invoice.AccountingCustomerParty) + "PostalZone", 50);
                                    if (_Headervalidation.Count > 0)
                                    {
                                        foreach (var error in _Headervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                }
                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingCustomerParty.Party.PostalAddress.CityName, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress.CityName), 50);
                                if (_Headervalidation.Count > 0)
                                {
                                    foreach (var error in _Headervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                                _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentityCode, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentityCode), 50);
                                if (!string.IsNullOrWhiteSpace(Invoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentityCode))
                                {
                                    _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(Invoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentityCode, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentityCode)));
                                    _Headervalidation.Add(new ValidationExtension().ValidateStates(Invoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentityCode, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress.CountrySubentityCode)));
                                }
                                if (_Headervalidation.Count > 0)
                                {
                                    foreach (var error in _Headervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                                if (Invoice.AccountingCustomerParty.Party.PostalAddress.Country.IdentificationCode != null)
                                {
                                    _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingCustomerParty.Party.PostalAddress.Country.IdentificationCode.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress.Country), 3);
                                    if (!string.IsNullOrWhiteSpace(Invoice.AccountingCustomerParty.Party.PostalAddress.Country.IdentificationCode.text))
                                    {
                                        _Headervalidation.Add(new ValidationExtension().ValidateCountry(Invoice.AccountingCustomerParty.Party.PostalAddress.Country.IdentificationCode.text, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress.Country)));
                                    }
                                    if (_Headervalidation.Count > 0)
                                    {
                                        foreach (var error in _Headervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                }
                                else
                                {
                                    ValidationErrors.Add(new ErrorModel
                                    {
                                        errMsg = nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress.Country) + " is mandatory.",
                                        PropertyName = nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PostalAddress.Country),
                                        PropertyValue = null
                                    });
                                }
                            }
                            else
                            {
                                ValidationErrors.Add(new ErrorModel
                                {

                                    errMsg = "AccountingCustomerParty PostalAddres is mandatory.",
                                    PropertyName = "AccountingCustomerParty PostalAddres",
                                    PropertyValue = "AccountingCustomerParty PostalAddres"
                                });
                            }
                            _Customervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PartyLegalEntity) + "/" + nameof(Invoice.AccountingCustomerParty.Party.PartyLegalEntity.RegistrationName), 300);
                            if (_Customervalidation.Count > 0)
                            {
                                foreach (var error in _Customervalidation)
                                {
                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                        ValidationErrors.Add(error);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Invoice.AccountingCustomerParty.Party.Contact.ElectronicMail))
                            {
                                _Customervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingCustomerParty.Party.Contact.ElectronicMail, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.Contact) + "/" + nameof(Invoice.AccountingCustomerParty.Party.Contact.ElectronicMail), 320);
                                if (!string.IsNullOrWhiteSpace(Invoice.AccountingCustomerParty.Party.Contact.ElectronicMail))
                                {
                                    _Customervalidation.Add(new ValidationExtension().ValidateEmailFormat(Invoice.AccountingCustomerParty.Party.Contact.ElectronicMail, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.Contact) + "/" + nameof(Invoice.AccountingCustomerParty.Party.Contact.ElectronicMail)));
                                }
                                if (_Customervalidation.Count > 0)
                                {
                                    foreach (var error in _Customervalidation)
                                    {
                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                            ValidationErrors.Add(error);
                                    }
                                }
                            }
                            _Headervalidation = new ValidationExtension().ValidatePropertySize(Invoice.AccountingCustomerParty.Party.Contact.Telephone, nameof(Invoice.AccountingCustomerParty) + "/" + nameof(Invoice.AccountingCustomerParty.Party) + "/" + nameof(Invoice.AccountingCustomerParty.Party.Contact) + "/" + nameof(Invoice.AccountingCustomerParty.Party.Contact.Telephone), 20);
                            if (_Headervalidation.Count > 0)
                            {
                                foreach (var error in _Headervalidation)
                                {
                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                        ValidationErrors.Add(error);
                                }
                            }
                        }
                        else
                        {
                            ValidationErrors.Add(new ErrorModel
                            {

                                errMsg = "AccountingCustomerParty cannot be empty.",
                                PropertyName = "AccountingCustomerParty",
                                PropertyValue = "AccountingCustomerParty"
                            });
                        }
                        #endregion
                        #region AdditionalDocumentReference/
                        var _AdditionalDocumentReference = new ValidationExtension().GetMultiElements<AdditionalDocumentReferenceMulti>(PathToXml, "AdditionalDocumentReference", true, ref errorMsg);
                        if (_AdditionalDocumentReference.Count > 0)
                        {
                            foreach (var _DocumentReference in _AdditionalDocumentReference)
                            {
                                if (_DocumentReference.AdditionalDocumentReference != null)
                                {
                                    _Headervalidation = new ValidationExtension().ValidatePropertyBySizeRange(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID", 3, 300);
                                    if (_Headervalidation.Count > 0)
                                    {
                                        foreach (var error in _Headervalidation)
                                        {
                                            if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                ValidationErrors.Add(error);
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(_DocumentReference.AdditionalDocumentReference.DocumentType))
                                        {
                                            _Headervalidation = new ValidationExtension().ValidatePropertySize(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID", 3);
                                            if (!string.IsNullOrWhiteSpace(_DocumentReference.AdditionalDocumentReference.ID))
                                            {
                                                _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID"));
                                                _Headervalidation.Add(new ValidationExtension().ValidateSpecialStrings(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID", ""));
                                            }
                                            if (_Headervalidation.Count > 0)
                                            {
                                                foreach (var error in _Headervalidation)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                        ValidationErrors.Add(error);
                                                }
                                            }
                                        }
                                        else if (_DocumentReference.AdditionalDocumentReference.DocumentType == "CustomsImportForm")
                                        {
                                            _Headervalidation = new ValidationExtension().ValidatePropertySize(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID", 19);
                                            if (!string.IsNullOrWhiteSpace(_DocumentReference.AdditionalDocumentReference.ID))
                                            {
                                                _Headervalidation.Add(new ValidationExtension().ValidateSpecialStrings(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID", ""));
                                            }
                                            if (_Headervalidation.Count > 0)
                                            {
                                                foreach (var error in _Headervalidation)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                        ValidationErrors.Add(error);
                                                }
                                            }
                                        }
                                        else if (_DocumentReference.AdditionalDocumentReference.DocumentType == "FreeTradeAgreement")
                                        {
                                            _Headervalidation = new ValidationExtension().ValidatePropertySize(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID", 3);
                                            if (!string.IsNullOrWhiteSpace(_DocumentReference.AdditionalDocumentReference.ID))
                                            {
                                                _Headervalidation.Add(new ValidationExtension().ValidateSpecialStrings(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID", "()-"));
                                                _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID"));
                                                if (_DocumentReference.AdditionalDocumentReference.ID != "FTA")
                                                {
                                                    _Headervalidation.Add(new ErrorModel
                                                    {
                                                        errMsg = "FreeTradeAgreement ID should be FTA.",
                                                        PropertyName = nameof(_DocumentReference.AdditionalDocumentReference.DocumentType),
                                                        PropertyValue = _DocumentReference.AdditionalDocumentReference.ID
                                                    });
                                                }
                                            }
                                            if (!string.IsNullOrWhiteSpace(_DocumentReference.AdditionalDocumentReference.DocumentDescription))
                                            {
                                                var temp = new ValidationExtension().ValidatePropertySize(_DocumentReference.AdditionalDocumentReference.DocumentDescription, nameof(_DocumentReference.AdditionalDocumentReference.DocumentDescription), 300);
                                                if (temp.Count > 0)
                                                {
                                                    foreach (var error in temp)
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                            ValidationErrors.Add(error);
                                                    }
                                                }
                                                _Headervalidation.Add(new ValidationExtension().ValidateSpecialStrings(_DocumentReference.AdditionalDocumentReference.DocumentDescription, nameof(_DocumentReference.AdditionalDocumentReference.DocumentDescription), "()-"));
                                            }
                                            if (_Headervalidation.Count > 0)
                                            {
                                                foreach (var error in _Headervalidation)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                        ValidationErrors.Add(error);
                                                }
                                            }
                                        }
                                        else if (_DocumentReference.AdditionalDocumentReference.DocumentType == "K2")
                                        {
                                            _Headervalidation = new ValidationExtension().ValidatePropertySize(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID", 12);
                                            _Headervalidation.Add(new ValidationExtension().ValidatePropertyNormalization(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID"));
                                            _Headervalidation.Add(new ValidationExtension().ValidateSpecialStrings(_DocumentReference.AdditionalDocumentReference.ID, nameof(_DocumentReference.AdditionalDocumentReference) + " ID", ""));
                                            if (_Headervalidation.Count > 0)
                                            {
                                                foreach (var error in _Headervalidation)
                                                {
                                                    if (!string.IsNullOrWhiteSpace(error.errMsg))
                                                        ValidationErrors.Add(error);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ValidationErrors.Add(new ErrorModel
                                            {
                                                errMsg = nameof(_DocumentReference.AdditionalDocumentReference.DocumentType) + " is invalid.",
                                                PropertyName = nameof(_DocumentReference.AdditionalDocumentReference.DocumentType)+ "-" + _DocumentReference.AdditionalDocumentReference.DocumentType,
                                                PropertyValue = string.Join(",", new List<string> { _DocumentReference.AdditionalDocumentReference.ID, _DocumentReference.AdditionalDocumentReference.DocumentType, _DocumentReference.AdditionalDocumentReference.DocumentDescription })
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        #endregion 
                    }
                    else
                    {
                        ValidationErrors.Add(new ErrorModel
                        {
                            errMsg = "Invoice is not valid.",
                            PropertyName = "Invoice",
                            PropertyValue = null
                        });
                    }
                }
                if (ValidationErrors.Count > 0)
                {
                    var groupedErrors = ValidationErrors.GroupBy(e => new { e.PropertyName }).Select(g => new ErrorModel
                    {
                        PropertyName = g.Key.PropertyName,
                        PropertyValue = g.Select(e => e.PropertyValue).FirstOrDefault(),
                        errMsg = string.Join(", ", g.Select(e => e.errMsg))
                    }).ToList();
                    return Json(new { errors = groupedErrors });
                }
                else
                {
                    return Json(new { status = true, message = "Your invoice is valid." });
                }
            }
            else
            {
                ErrorModel ErrorObj = new ErrorModel();
                ErrorObj.errMsg = "Select a file to validate.";
                ValidationErrors.Add(ErrorObj);
                return Json(new { errors = ValidationErrors });
            }
        }
        private List<string> GetXmlFiles(string PathToXml)
        {
            List<string> xml = new List<string>{
                PathToXml,
            };
            return xml;
        }
    }
}