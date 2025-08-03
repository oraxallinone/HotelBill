$(document).ready(function () {
    console.clear();
    hidelFooterGstGroup();
    let isDraft = ($("#IsDraft").val())== 'False' ? false:true;
   
    if (isDraft) {
        loadDraft();
    }
    else {
        $("#IDate").val(formatDate());
    }

    $('[id^=itm_Quantity]').keypress(validateNumber);


    //load draft
    function loadDraft() {
        $.ajax({
            url: '/Invoice/GetDraftData',
            type: "GET",
            dataType: "json",
            contentType: 'application/json',
            data: { draftNo: $("#IDraftNo").val() },
            success: function (result) {
                console.log(result);
                if (result) {
                    //for each loop
                    slno = slno + 1;
                    let bodyHtml = "";

                    $("#custId").val(result.ICustId);
                    $("#IDate").val(formatDate2(result.IDate));

                    $.each(result.ItemTransactions, function (index, value) {
                        bodyHtml = bodyHtml + " <tr class='fontMenuBody tableBorderitem cTableRow bg-bisq'>" +
                                         "<td class='textCenter cItmCode'>" + value.ItemCodeT + "</td>" +
                                         "<td class='cItmName'>" + value.ItemDetails + " </td>" +
                                         "<td class='textCenter cItmHsn'>" + value.HSN + "</td>" +
                                         "<td class='textCenter cItmQty'>" + value.Quantity + "</td>" +
                                         "<td class='textCenter cItmRate'>" + value.Rate + "</td>" +
                                         "<td class='textRight cItmValue'>" + value.Value + "</td>" +
                                         "<td class='textCenter cItmGst'>" + value.GstRate + "</td>" +
                                         "<td class='textRight cItmGstValue'>" + value.GstValue + "</td>" +
                                         "<td class='textRight cItmTotal'>" + value.Total + "</td>" +
                                         "<td class='textCenter'>" +
                                               "<input type='button' class='fa-solid fa-trash-can fontRed btmItmDelete' /> </td>" +
                                         "</tr>";
                    });

                   


                    $("#tblInvoice tbody").append(bodyHtml);
                    calulateValue();
                    gstCalculation();
                    resetAdd();
                }



            }
        });
    }

    //get item detais by id
    $("#ItemCode").change(function () {
        let selectedItemId = $('select#ItemCode option:selected').val();
        let itemID = parseInt(selectedItemId);
        $.ajax({
            type: "POST",
            url: '/Invoice/GetItmById',
            data: { id: itemID },
            dataType: "json",
            success: function (data) {
                $("#itm_ItemCodeT").val(data.ItemCode);
                $("#itm_ItemDetails").val(data.ItemDetails);
                $("#itm_HSN").val(data.HSN);
                $("#itm_Rate").val(data.Rate);
                $("#itm_GstRate").val(data.Gst);
            }
        });
    });

    //on key up do calculation
    $("#itm_Quantity").keyup(function () {
        taxCalculation();
    });

    $(document).on('click', '#iMinus', function (e) {
        let itmName = $('select#ItemCode option:selected').text();
        if (itmName == "-- Select One --") { swal("", "Please select an item", "error"); return false; }
        let value = $("#itm_Quantity").val();
        if (value == "" || parseInt(value) == 0) { swal("", "Please add a quantity", "info"); return false; }
        if (value != "" || parseInt(value) > 0) { $("#itm_Quantity").val(parseInt(value) - 1); }
        taxCalculation();
    });

    $(document).on('click', '#iPlus', function (e) {
        let itmName = $('select#ItemCode option:selected').text();
        if (itmName == "-- Select One --") { swal("", "Please select an item", "error"); return false; }
        let value = $("#itm_Quantity").val();
        if (value == "") { $("#itm_Quantity").val(1); }
        if (value != "") { $("#itm_Quantity").val(parseInt(value) + 1); }
        taxCalculation();
    });

    //add multiple item to table
    let slno = 0;
    $("#btnAddItm").click(function () {
        let custId = $('select#custId option:selected').val(); if (custId == "") { swal("", "Please choose customer first", "error"); return false; }
        let itmId = $('select#ItemCode option:selected').val();
        let itmName = $('select#ItemCode option:selected').text();
        let itmHsn = $("#itm_HSN").val();
        let itmQty = $("#itm_Quantity").val();
        let itmRate = $("#itm_Rate").val();
        let itmValue = $("#itm_Value").val();
        let itmGst = $("#itm_GstRate").val();
        let itmGstValue = $("#itm_GstValue").val();
        let itmTotal = $("#itm_Total").val();

        if (itmName == "-- Select One --") { swal("", "Please select an item", "error"); return false; }//custId
        if (itmHsn == "") { swal("", "HSN should not be empty", "error"); return false; }
        if (itmQty == "") { swal("", "Qantity should not be empty", "error"); return false; }
        if (parseInt(itmQty) < 1) { swal("", "Please add a quantity", "error"); return false; }
        if (itmRate == "") { alert("Rate should not be empty"); return false; }
        if (itmValue == "" || itmValue == 0) { alert("Value should not be empty"); return false; }

        slno = slno + 1;
        let bodyHtml = " <tr class='fontMenuBody tableBorderitem cTableRow'>" +
                              "<td class='textCenter cItmCode'>" + itmId + "</td>" +
                              "<td class='cItmName'>" + itmName + " </td>" +
                              "<td class='textCenter cItmHsn'>" + itmHsn + "</td>" +
                              "<td class='textCenter cItmQty'>" + itmQty + "</td>" +
                              "<td class='textCenter cItmRate'>" + itmRate + "</td>" +
                              "<td class='textRight cItmValue'>" + itmValue + "</td>" +
                              "<td class='textCenter cItmGst'>" + itmGst + "</td>" +
                              "<td class='textRight cItmGstValue'>" + itmGstValue + "</td>" +
                              "<td class='textRight cItmTotal'>" + itmTotal + "</td>" +
                              "<td class='textCenter'>" +
                                    "<input type='button' class='fa-solid fa-trash-can fontRed btmItmDelete' /> </td>" +
                              "</tr>";

        $("#tblInvoice tbody").append(bodyHtml);
        calulateValue();
        gstCalculation();
        resetAdd();
    });

    //remove added items
    $(document).on("click", '.btmItmDelete', function () {
        if (!confirm("Do you want to delete")) { return false; }
        $(this).closest('tr').remove();
        calulateValue();
        gstCalculation();
        swal("Item Removed", "", "success");
    });


    $('#reset').click(function () {
        location.reload();
    });
    $('#custId').change(function () {
        let gstType = ($(this).val()).split('-')[1];
        if (gstType == "1")//cgst
        {
            $(".section-group").show();
            $(".section-sgst").show();
            $(".section-igst").hide();
        }
        else if (gstType == "2") {//igst
            $(".section-group").show();
            $(".section-sgst").hide();
            $(".section-igst").show();
        }
        else {
            hidelFooterGstGroup();
        }

        if (isDraft) {
            calulateValue();
            gstCalculation();
            resetAdd();
        }
    });

    //invoice submit
    $('#invoiceSubmit').click(function () {
        invoiceCreate('submitAndNew');
    });

    $('#invoicePrint').click(function () {
        invoiceCreate('submitAndPrint');
    });
    $('#invoiceDraft').click(function () {
        invoiceCreate('draft');
    });

    function invoiceCreate(type) {
        let ICustId = $('select#custId option:selected').val(); if (ICustId == "") { swal("", "Please choose customer first", "error"); return false; }//custId
        let drafteNo = $('#IDraftNo').val();
        let invoiceNo = $('#INo').val();
        let invoiceDate = $('#IDate').val();
        let invoiceGrand_Value = $('.gItmValue').text();
        let invoiceGrand_Gst_Value = $('.gItmGstValue').text();
        let invoiceGrand_total = $('.gItmTotal').text();
        if ($('#tblInvoice tbody > tr').length < 1) { swal("", "Please add atlist one item to create invoice", "error"); return false; }
        if (drafteNo == "") { swal("", "draft should not be empty", "error"); return false; }
        if (invoiceNo == "") { swal("", "invoiceNo should not be empty", "error"); return false; }
        if (invoiceDate == "") { swal("", "invoiceDate should not be empty", "error"); return false; }
        if (invoiceGrand_Value == "0.00") { swal("", "invoice Value should not be zero", "error"); return false; }
        if (invoiceGrand_Gst_Value == "0.00") { swal("", "invoice Gst should not be zero", "error"); return false; }
        if (invoiceGrand_total == "0.00") { swal("", "invoice total should not be zero", "error"); return false; }

        let addedItemList = [];
        let count = 0;

        $('#tblInvoice tbody > tr').each(function (index, tr) {
            let itm_code = $(this).find('td.cItmCode').text();
            let itm_name = $(this).find('td.cItmName').text();
            let itm_hsn = $(this).find('td.cItmHsn').text();
            let itm_qty = $(this).find('td.cItmQty').text();
            let itm_rate = $(this).find('td.cItmRate').text();
            let itm_value = $(this).find('td.cItmValue').text();
            let itm_gst = $(this).find('td.cItmGst').text();
            let itm_gstValue = $(this).find('td.cItmGstValue').text();//new
            let itm_total = $(this).find('td.cItmTotal').text();
            let singItem = {
                itm_ItemCodeT: itm_code,
                itm_ItemDetails: itm_name,
                itm_HSN: itm_hsn,
                itm_Quantity: itm_qty,
                itm_Rate: itm_rate,
                itm_Value: itm_value,
                itm_GstRate: itm_gst,
                itm_GstValue: itm_gstValue,//new
                itm_Total: itm_total
            }
            addedItemList.push(singItem);
        });

        let invoiceStatus = (type == "draft") ? "draft" : "invoice";

        let InvoiceGstViewModel = {
            INo: invoiceNo,
            IinvoiceStatus: invoiceStatus,
            IDraftNo: drafteNo,
            IDate: invoiceDate,
            ICustId: ICustId.split("-")[0],
            TotalValue: invoiceGrand_Value,
            TotalGST: invoiceGrand_Gst_Value,
            TotalTotal: invoiceGrand_total,
            ItemTransList: addedItemList
        }
        $.ajax({
            url: '/Invoice/GstInvoiceCreate',
            type: "POST",
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(InvoiceGstViewModel),
            success: function (data) {
                if (data != 0) {
                    let createdInvoiceNo = $('#INo').val();
                    let createdDraftNo = $('#IDraftNo').val();

                    $('#custId').val('');
                    hidelFooterGstGroup();
                    $('.re-set').html('0.00');
                    resetPageAfterSave();
                    //& draft
                    if (type == 'draft') {
                        swal("draft successfully created", createdDraftNo, "warning");
                        $('#IDraftNo').val(data.newDraftNo);
                        $('#INo').val(data.newInvoiceNo);

                    }
                    //& new
                    if (type == 'submitAndNew') {
                        swal("Invoice successfully created", createdInvoiceNo, "success");
                        $('#IDraftNo').val(data.newDraftNo);
                        $('#INo').val(data.newInvoiceNo);

                    }
                    //& print
                    if (type == 'submitAndPrint') {
                        window.location.href = '/Invoice/GstInvoicePrint?INo=' + createdInvoiceNo;
                    }
                }
                else {
                    swal("Invoice not created", "invoiceNo", "error");
                }
            }
        });
    }

    //functions list
    function hidelFooterGstGroup() {
        $(".section-group").hide();
        $(".section-sgst").hide();
        $(".section-igst").hide();
    }

    function taxCalculation() {
        let qty = parseInt($("#itm_Quantity").val());
        let rate = parseFloat($("#itm_Rate").val());
        let gst = parseFloat($("#itm_GstRate").val());
        let value = qty * rate;
        let taxAble = (gst * value) / 100;
        let halfTax = taxAble / 2;
        let taotal = value + taxAble;
        $("#itm_Value").val(value.toFixed(2));
        $("#itm_GstValue").val(taxAble.toFixed(2));
        $("#itm_Total").val(Math.round(taotal.toFixed(2)));
    };

    function gstCalculation() {
        let gst5 = 0.00, gst12 = 0.00, gst18 = 0.00, gstAll = 0.00;

        $('.cTableRow').each(function () {
            let gstGroup = $(this).find("td:eq(6)").text();
            let gstValue = $(this).find("td:eq(7)").text();

            gstAll = (parseFloat(gstAll) + parseFloat(gstValue)).toFixed(2);

            if (gstGroup == "5") {
                gst5 = (parseFloat(gst5) + parseFloat(gstValue)).toFixed(2);
            }
            else if (gstGroup == "12") {
                gst12 = (parseFloat(gst12) + parseFloat(gstValue)).toFixed(2);
            }
            else if (gstGroup == "18") {
                gst18 = (parseFloat(gst18) + parseFloat(gstValue)).toFixed(2);
            }
        });

        let gstTyep = $('#custId').val().split("-")[1];
        if (gstTyep == "1") {
            //cgst
            $('#cgst-5-footer').text((gst5 / 2).toFixed(2));
            $('#cgst-12-footer').text((gst12 / 2).toFixed(2));
            $('#cgst-18-footer').text((gst18 / 2).toFixed(2));
            $('#cgst-all-footer').text((gstAll / 2).toFixed(2));
            //sgst
            $('#sgst-5-footer').text((gst5 / 2).toFixed(2));
            $('#sgst-12-footer').text((gst12 / 2).toFixed(2));
            $('#sgst-18-footer').text((gst18 / 2).toFixed(2));
            $('#sgst-all-footer').text((gstAll / 2).toFixed(2));
        }
        if (gstTyep == "2") {
            //igst
            $('#igst-12-footer').text(gst12);
            $('#igst-18-footer').text(gst18);
            $('#igst-all-footer').text(gstAll);
            $('#igst-5-footer').text(gst5);
        }
        //gst hide
        if (gst5 == 0) { $('.gst-5-show').hide(); } else { $('.gst-5-show').show(); }
        if (gst12 == 0) { $('.gst-12-show').hide(); } else { $('.gst-12-show').show(); }
        if (gst18 == 0) { $('.gst-18-show').hide(); } else { $('.gst-18-show').show(); }
    }
     

    function resetPageAfterSave() {
        $("#tblInvoice tbody").empty();
        calulateValue();
    }

    function calulateValue() {
        let finalgItmValue = 0.00;
        let finalgItmGst = 0.00;
        let finalggItmTotal = 0.00;

        if ($("#tblInvoice tbody>tr").length > 0) {
            $('.cItmValue').each(function () {
                finalgItmValue = finalgItmValue + parseFloat($(this).html());
                $('.gItmValue').html(Math.round(finalgItmValue).toFixed(2));
            });

            $('.cItmGstValue').each(function () {
                finalgItmGst = finalgItmGst + parseFloat($(this).html());
                $('.gItmGstValue').html(Math.round(finalgItmGst).toFixed(2));
            });

            $('.cItmTotal').each(function () {
                finalggItmTotal = finalggItmTotal + parseFloat($(this).html());
                $('.gItmTotal').html(Math.round(finalggItmTotal).toFixed(2));
            });
        }
        else {
            $('.gItmValue').html(finalgItmValue.toFixed(2));
            $('.gItmGstValue').html(finalgItmGst.toFixed(2));
            $('.gItmTotal').html(finalggItmTotal.toFixed(2));
        };
    }

    function resetAdd() {
        $("#ItemCode").val($("#ItemCode option:first").val());
        $("#itm_HSN").val('');
        $("#itm_Quantity").val('');
        $("#itm_Rate").val('');
        $("#itm_Value").val('');
        $("#itm_GstRate").val('');
        $("#itm_GstValue").val('');
        $("#itm_Total").val('');
    }
});