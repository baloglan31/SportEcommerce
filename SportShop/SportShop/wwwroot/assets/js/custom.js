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


const updateQtyBtns = document.querySelectorAll('.btn-update-qty');

updateQtyBtns.forEach(function (btn) {
    btn.addEventListener('click', function (e) {
        e.preventDefault();

        const productId = this.getAttribute('data-id');
        const change = parseInt(this.getAttribute('data-change'));

        fetch('/Cart/UpdateQuantity', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({ productId: parseInt(productId), change: change })
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    const cartCounter = document.getElementById('cart-counter');
                    if (cartCounter) cartCounter.innerText = data.cartCount;

                    if (data.currentQuantity === 0) {
                        const row = document.getElementById(`cart-row-${productId}`);
                        if (row) row.remove();
                        toastr.info("Item is removed from cart.");

                        if (data.cartCount === 0) location.reload();
                    }

                    else {
                        document.querySelector(`.qty-input-${productId}`).value = data.currentQuantity;
                        document.querySelector(`.item-total-${productId}`).innerText = data.itemTotal;
                    }


                    const summaryCount = document.getElementById('cart-summary-count');
                    const summaryTotal = document.getElementById('cart-summary-total');

                    if (summaryCount) summaryCount.innerText = data.cartCount + ' ədəd';
                    if (summaryTotal) summaryTotal.innerText = data.cartTotal;

                }
            })
            .catch(error => console.error('Error:', error));
    });
});