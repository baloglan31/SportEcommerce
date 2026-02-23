document.addEventListener('DOMContentLoaded', function () {
    toastr.options = {
        "closeButton": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "timeOut": "3000"
    };

    const addToCartBtns = document.querySelectorAll('.btn-add-to-cart');

    addToCartBtns.forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();

            const productId = this.getAttribute('data-product-id');
            const requestData = {
                productId: parseInt(productId),
                quantity: 1
            };

            fetch('/Cart/AddToCart', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(requestData)
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        const cartCounter = document.getElementById('cart-counter');
                        if (cartCounter) {
                            cartCounter.innerText = data.cartCount;
                        }

                        
                        toastr.success(data.message, "Success!");

                    } else {
                        
                        toastr.error(data.message, "Something went wrong. Please try again!");
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    toastr.error("Something went wrong in system.", "Error!");
                });
        });
    });
});