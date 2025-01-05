// Debounce function to limit AJAX calls
function debounce(func, wait) {
    let timeout;
    return function (...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), wait);
    };
}

// Search books function
function searchBooks() {
    const searchQuery = document.getElementById('searchQuery').value;
    const category = document.getElementById('category').value;

    console.log("Search Query:", searchQuery); // Debugging
    console.log("Category:", category); // Debugging

    $('#loading').show();
    $.ajax({
        url: '/Book/UserIndex',
        type: 'GET',
        data: { category: category, searchQuery: searchQuery },
        success: function (result) {
            console.log("AJAX Success:", result); // Debugging
            $('#bookList').html(result);
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", error); // Debugging
            alert("An error occurred while fetching books. Please try again.");
        },
        complete: function () {
            $('#loading').hide();
        }
    });
}

// Clear filters function
function clearFilters() {
    document.getElementById('category').value = '';
    document.getElementById('searchQuery').value = '';
    searchBooks();
}

// Initialize countdown timers
function initCountdown(bookId, discountEndDate) {
    const countdownElement = document.getElementById(`countdown-${bookId}`);
    const endDate = new Date(discountEndDate).getTime();

    const timer = setInterval(() => {
        const now = new Date().getTime();
        const timeLeft = endDate - now;

        if (timeLeft <= 0) {
            clearInterval(timer);
            countdownElement.textContent = "Discount expired!";
            return;
        }

        const days = Math.floor(timeLeft / (1000 * 60 * 60 * 24));
        const hours = Math.floor((timeLeft % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((timeLeft % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((timeLeft % (1000 * 60)) / 1000);

        countdownElement.textContent = `${days}d ${hours}h ${minutes}m ${seconds}s`;
    }, 1000);
}

// Initialize all countdowns on page load
document.addEventListener('DOMContentLoaded', function () {
    // Initialize countdowns for discount timers
    document.querySelectorAll('[data-end-date]').forEach(element => {
        const bookId = element.id.replace('countdown-', '');
        const discountEndDate = element.getAttribute('data-end-date');
        initCountdown(bookId, discountEndDate);
    });

    // Attach debounced search to input
    const searchInput = document.getElementById('searchQuery');
    if (searchInput) {
        const debouncedSearchBooks = debounce(searchBooks, 300);
        searchInput.addEventListener('input', debouncedSearchBooks);
    }

    // Attach clear filters button
    const clearFiltersButton = document.querySelector('button[onclick="clearFilters()"]');
    if (clearFiltersButton) {
        clearFiltersButton.addEventListener('click', clearFilters);
    }
});