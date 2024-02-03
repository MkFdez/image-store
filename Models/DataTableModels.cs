using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Pagination
    {
        public DatatablePostData data { get; set; }
    }
    public class DatatablePostData
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
    }

    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public string searchable { get; set; }
        public string orderable { get; set; }
        public Search search { get; set; }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
    public class DTResponse
    {
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public string data { get; set; }

        public string collections { get; set; }
    }
}