namespace SmartPharma5.Model
{
    public class Partner_Form_Response
    {
        public int Id { get; set; }
        public int question_id { get; set; }
        public int response_id { get; set; }
        public int partner_form_id { get; set; }

        public Partner_Form_Response()
        {
        }
        public Partner_Form_Response(int question_id, int response_id, int partner_form_id)
        {
            this.question_id = question_id;
            this.response_id = response_id;
            this.partner_form_id = partner_form_id;
        }
    }
}
