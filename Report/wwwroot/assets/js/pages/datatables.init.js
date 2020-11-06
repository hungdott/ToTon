$(document).ready(function () {
   var table = $("#datatable").DataTable({
       responsive: true,
      // fixedHeader: true
   });
   new $.fn.dataTable.FixedHeader(table);

});