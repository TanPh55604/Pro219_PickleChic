namespace PickleChic.WEB.Model
{
    public class ProductVariantModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string? VariantName { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public int Status { get; set; } = 1;

        public bool IsActive => Status == 1;
    }

    public class ProductVariantAttributePickerModel
    {
        public int AttributeId { get; set; }

        public string AttributeName { get; set; } = string.Empty;

        public int SelectedValueId { get; set; }

        public List<AttributeValueOptionModel> Values { get; set; } = new();
    }

    public class AttributeValueOptionModel
    {
        public int Id { get; set; }

        public string Value { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
}
