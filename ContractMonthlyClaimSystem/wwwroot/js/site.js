// JavaScript for dynamic functionality
document.addEventListener('DOMContentLoaded', function () {
    // Calculate total amount
    const calculateTotal = () => {
        let total = 0;
        document.querySelectorAll('input[name$=".Amount"]').forEach(input => {
            total += parseFloat(input.value) || 0;
        });
        document.getElementById('totalAmount').textContent = '$' + total.toFixed(2);
    };

    // Add event listeners for amount changes
    document.addEventListener('input', function (e) {
        if (e.target.name && e.target.name.includes('.Amount')) {
            calculateTotal();
        }
    });

    // Initial calculation
    calculateTotal();
});