namespace TixGurusWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Role = "\"Member\"";
            this.Bookings = new HashSet<Booking>();
        }

        public int UserID { get; set; }
        [Required(ErrorMessage = "Please Provide Username", AllowEmptyStrings = false)]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "Please Provide Password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Role { get; set; }
        public string LoginErrorMessage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
