$(document).ready(function () {
    var loadingData = false;
    try {
        // Initialize DataTable
        dataTable = $("#datatable-buttons").DataTable({
            scrollY: '100vh', // Enable vertical scrolling
            lengthChange: true,
            pageLength: 100,
            initComplete: function () {
                // show the table after datatables is fully initialized
                $('#datatableContainer0').delay(10).show();
                $.fn.dataTable.ext.errMode = 'none'; // Disable error reporting for timeout
                $.ajaxSetup({
                    timeout: 0, // Set timeout to 0 for infinite timeout
                });
            },
            dom: '<"row"<"col-md-6"B><"col-md-6"f>>rt<"row"<"col-md-6"l><"col-md-6"p>>',
            buttons: [
                {
                    extend: 'excelHtml5',
                    text: 'Excel',
                    className: 'btn btn-success btn-sm waves-effect waves-light',
                    filename: function () {
                        // Extract searchMailNumber, searchSubject, startDate, and endDate from the URL parameters
                        var urlParams = new URLSearchParams(window.location.search);
                        var searchMailNumber = urlParams.get('searchMailNumber');
                        var searchSubject = urlParams.get('searchSubject');
                        var startDate = urlParams.get('startDate');
                        var endDate = urlParams.get('endDate');

                        // Check if searchMailNumber is present, if yes, use it for filename
                        if (searchMailNumber && searchMailNumber.trim() !== '') {
                            return searchMailNumber + '_ElasticData';
                        } else if (searchSubject && searchSubject.trim() !== '') {
                            // If searchMailNumber is not present, check searchSubject for filename
                            return searchSubject + '_ElasticData';
                        } else if (startDate && startDate.trim() !== '' && endDate && endDate.trim() !== '') {
                            // If both startDate and endDate are present, format them for filename
                            var formattedStartDate = formatDate(startDate);
                            var formattedEndDate = formatDate(endDate);

                            return formattedStartDate + ' to ' + formattedEndDate + '_ElasticData';
                        } else {
                            // Default filename if none of the parameters are present
                            return 'ElasticReport';
                        }
                    }
                },
            ],
            language: {
                search: "Table Filter:"
            },
            order: [
                [
                   5, 'asc'
                ]
            ],
            columnDefs: [{
                targets: 0,
                orderable: false
            }],
            // Callbacks to show/hide loading modal
            "preInit": function () {
                showLoadingModal();
            },
            "drawCallback": function () {
                hideLoadingModal();
            }
        });
        setTimeout(function () {
            $($.fn.dataTable.tables(true)).DataTable().columns.adjust().draw();
        }, 10);

        // Include DataTable buttons in a separate div for better styling
        $('.dt-buttons').appendTo('.buttons-container');

        document.getElementById('searchMailNumberInput').addEventListener('input', function () {
            /* Automatically submit the form when the mail number input changes*/
            document.getElementById('mailNumberForm').submit();
        });

        document.forms["searchForm"].addEventListener('blur', async function (event) {
            // Prevent the default form submission behavior
            event.preventDefault();

            // Get the current value of the hidden input for searchMailNumber
            var searchMailNumber = document.forms["searchForm"]["searchMailNumber"].value;
            // If searchMailNumber is not empty, fetch and display the data
            if (searchMailNumber.trim() !== '') {
                await loadDataAndDisplayTable(searchMailNumber);
                // Load and display DataTable after fetching data
                loadDataTable();
            }
        });

        document.getElementById('searchSubjectInput').addEventListener('input', async function () {
            // Get the current value of the searchSubject input
            var searchSubject = document.forms["searchForm"]["searchSubject"].value;

            // If searchSubject is not empty, fetch and display the data
            if (searchSubject.trim() !== '') {
                try {
                    showLoadingModal(); // Show loading modal before starting the fetch

                    // Construct the URL for fetching data based on the searchSubject
                    var url = '/Home/DisplayElasticData?searchSubject=' + encodeURIComponent(searchSubject);

                    const response = await fetch(url);

                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }

                    const data = await response.json();
                    console.log(data);

                    // Hide the loading modal after the data is loaded and displayed
                    hideLoadingModal();
                } catch (error) {
                    console.error('Error:', error);
                    hideLoadingModal(); // Hide the loading modal in case of an error
                }
                // Load and display DataTable after fetching data
                loadDataTable();
            }
        });

        document.getElementById('searchButton').addEventListener('click', function () {
            handleButtonClick(loadDataAndDisplayTable_);
        });

        document.getElementById('filterButton').addEventListener('click', function () {
            handleButtonClick(loadDataAndDisplayTable);
        });

        async function handleButtonClick(callback) {
            if (!loadingData) {
                loadingData = true;
                await callback(); // Wait for the callback to complete
                loadingData = false;
            }
        }

        document.getElementById('mailNumberForm').addEventListener('blur', async function (event) {
            // Prevent the default form submission behavior
            event.preventDefault();

            // Get the current value of the mail number input
            var mailNumber = document.getElementById('searchMailNumberInput').value;

            // If the mail number is not empty, fetch and display the data
            if (mailNumber.trim() !== '') {
                await loadDataAndDisplayTable(mailNumber);
                // Load and display DataTable after fetching data
                loadDataTable();
            }
        });
    } catch (error) {
        console.error("An error occurred:", error);
    }
});

// Function to show the loading modal
function showLoadingModal() {
    var loadingModal = document.getElementById('ProgressModal');
    loadingModal.style.display = "block";
}

// Function to hide the loading modal
function hideLoadingModal() {
    var loadingModal = document.getElementById('ProgressModal');
    loadingModal.style.display = "none";
}

function formatDate(dateString) {
    var date = new Date(dateString);
    var month = (date.getMonth() + 1).toString().padStart(2, '0');
    var day = date.getDate().toString().padStart(2, '0');
    var year = date.getFullYear().toString().substring(-2);

    return month + '-' + day + '-' + year;
}

// --- MAIL NUMBER ---
async function loadDataAndDisplayTable_(searchMailNumber) {
    try {
        var searchSubjectInput = document.getElementById('searchSubjectInput').value;

        // Check if the input is null or empty
        if (!searchSubjectInput) {
            alert('Field cannot be empty.'); // Display an error message
            return; // Stop script execution
        }
        showLoadingModal(); // Show loading modal before starting the fetch

        // Construct the URL for fetching data based on the mail number
        var url = `/Home/DisplayElasticData?searchMailNumbers=${searchMailNumber}`;

        const response = await fetch(url);

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();
        console.log(data);
        // Hide the loading modal after the data is loaded and displayed
        hideLoadingModal();

    } catch (error) {
        console.error('Error:', error.message);
        console.error('Stack Trace:', error.stack);
        const responseText = await error.response.text(); // Use error.response here
        console.log('Response Text:', responseText);
        hideLoadingModal(); // Hide the loading modal in case of an error
    }
}

// --- DATE FILTER ---
async function loadDataAndDisplayTable() {
    try {
        showLoadingModal(); // Show the loading modal before the fetch request

        var startDate = document.getElementById('startDateInput').value;
        var endDate = document.getElementById('endDateInput').value;
        var url = '/Home/DisplayElasticData?startDate=' + startDate + '&endDate=' + endDate + '&filterButton=';

        const response = await fetch(url);

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();
        console.log(data);
        // Hide the loading modal after the data is loaded and displayed
        hideLoadingModal();

    } catch (error) {
        console.error('Error:', error.message);
        console.error('Stack Trace:', error.stack);
        const responseText = await error.response.text(); // Use error.response here
        console.log('Response Text:', responseText);
        hideLoadingModal(); // Hide the loading modal in case of an error
    }
}
// --- CODE DUMP ---
// --- SUBJECT FILTER ---
//async function loadDataAndDisplayTable_() {
//    try {
//        showLoadingModal(); // Show loading modal before starting the fetch

//        var searchSubject = document.getElementById('startDateInput').value;
//        var url = `/Home/DisplayElasticData?searchMailNumber=${searchSubject}`;

//        const response = await fetch(url);

//        if (!response.ok) {
//            throw new Error('Network response was not ok');
//        }

//        const data = await response.json();

//        // Update your table or do other processing here with the data
//        console.log(data);

//    } catch (error) {
//        console.error('Error:', error);
//        hideLoadingModal();
//    }
//}

//// Scroll to the bottom of the table
//$("#goToBottomBtn").click(function () {
//    var tableBottom = $("#datatable-buttons").offset().top + $("#datatable-buttons").height();
//    $("html, body").animate({ scrollTop: tableBottom }, 500);
//});

//// Return to Top button click event
//$("#returnToTopBtn").click(function () {
//    $("html, body").animate({ scrollTop: 0 }, 500);
//});
/*scrollX: true, */   // Enable horizontal scrolling
/* fixedHeader: true,*/ // Fix the table header

//$(document).ready(function () {
//    var loadingData = false;
//    try {
//        // Initialize DataTable
//        dataTable = $("#datatable-buttons").DataTable({
//            scrollY: '100vh', // Enable vertical scrolling
//            lengthChange: true,
//            pageLength: 100,
//            initComplete: function () {
//                // show the table after datatables is fully initialized
//                $('#datatableContainer0').delay(10).show();
//            },
//            dom: '<"row"<"col-md-6"B><"col-md-6"f>>rt<"row"<"col-md-6"l><"col-md-6"p>>',
//            buttons: [
//                {
//                    extend: 'excelHtml5',
//                    text: 'Excel',
//                    className: 'btn btn-success btn-sm waves-effect waves-light',
//                    filename: 'ElasticReport'
//                },
//            ],
//            language: {
//                search: "Table Filter:"
//            },
//            columnDefs: [{
//                targets: 0,
//                orderable: false
//            }],
//            // Callbacks to show/hide loading modal
//            "preInit": function () {
//                showLoadingModal();
//            },
//            "drawCallback": function () {
//                hideLoadingModal();
//            }
//        });
//        setTimeout(function () {
//            $($.fn.dataTable.tables(true)).DataTable().columns.adjust().draw();
//        }, 10);

//        // Include DataTable buttons in a separate div for better styling
//        $('.dt-buttons').appendTo('.buttons-container');

//        document.getElementById('searchMailNumberInput').addEventListener('input', function () {
//            /* Automatically submit the form when the mail number input changes*/
//            document.getElementById('mailNumberForm').submit();
//        });

//        document.forms["searchForm"].addEventListener('blur', async function (event) {
//            // Prevent the default form submission behavior
//            event.preventDefault();

//            // Get the current value of the hidden input for searchMailNumber
//            var searchMailNumber = document.forms["searchForm"]["searchMailNumber"].value;
//            // If searchMailNumber is not empty, fetch and display the data
//            if (searchMailNumber.trim() !== '') {
//                await loadDataAndDisplayTable(searchMailNumber);
//                // Load and display DataTable after fetching data
//                loadDataTable();
//            }
//        });

//        document.getElementById('searchSubjectInput').addEventListener('input', async function () {
//            // Get the current value of the searchSubject input
//            var searchSubject = document.forms["searchForm"]["searchSubject"].value;

//            // If searchSubject is not empty, fetch and display the data
//            if (searchSubject.trim() !== '') {
//                try {
//                    showLoadingModal(); // Show loading modal before starting the fetch

//                    // Construct the URL for fetching data based on the searchSubject
//                    var url = '/Home/DisplayElasticData?searchSubject=' + encodeURIComponent(searchSubject);

//                    const response = await fetch(url);

//                    if (!response.ok) {
//                        throw new Error('Network response was not ok');
//                    }

//                    const data = await response.json();
//                    console.log(data);

//                    // Hide the loading modal after the data is loaded and displayed
//                    hideLoadingModal();
//                } catch (error) {
//                    console.error('Error:', error);
//                    hideLoadingModal(); // Hide the loading modal in case of an error
//                }
//                // Load and display DataTable after fetching data
//                loadDataTable();
//            }
//        });

//        document.getElementById('searchButton').addEventListener('click', function () {
//            handleButtonClick(loadDataAndDisplayTable_);
//        });

//        document.getElementById('filterButton').addEventListener('click', function () {
//            handleButtonClick(loadDataAndDisplayTable);
//        });

//        function handleButtonClick(callback) {
//            if (!loadingData) {
//                loadingData = true;
//                callback();
//                loadingData = false;
//            }
//        }
//        document.getElementById('mailNumberForm').addEventListener('blur', async function (event) {
//            // Prevent the default form submission behavior
//            event.preventDefault();

//            // Get the current value of the mail number input
//            var mailNumber = document.getElementById('searchMailNumberInput').value;

//            // If the mail number is not empty, fetch and display the data
//            if (mailNumber.trim() !== '') {
//                await loadDataAndDisplayTable(mailNumber);
//                // Load and display DataTable after fetching data
//                loadDataTable();
//            }
//        });
//    } catch (error) {
//        console.error("An error occurred:", error);
//    }
//});

//// Function to show the loading modal
//function showLoadingModal() {
//    var loadingModal = document.getElementById('ProgressModal');
//    loadingModal.style.display = "block";
//}

//// Function to hide the loading modal
//function hideLoadingModal() {
//    var loadingModal = document.getElementById('ProgressModal');
//    loadingModal.style.display = "none";
//}

//// --- MAIL NUMBER ---
//function loadDataAndDisplayTable_(searchMailNumber) {
//    try {
//        showLoadingModal(); // Show loading modal before starting the fetch

//        // Construct the URL for fetching data based on the mail number
//        var url = `/Home/DisplayElasticData?searchMailNumbers=${searchMailNumber}`;

//        const response = await fetch(url);

//        if (!response.ok) {
//            throw new Error('Network response was not ok');
//        }

//        const data = await response.json();
//        console.log(data);
//        // Hide the loading modal after the data is loaded and displayed
//        hideLoadingModal();

//    } catch (error) {
//        console.error('Error:', error.message);
//        console.error('Stack Trace:', error.stack);
//        const responseText = await response.text();
//        console.log('Response Text:', responseText);
//        hideLoadingModal(); // Hide the loading modal in case of an error
//    }
//}

//// --- DATE FILTER ---
//function loadDataAndDisplayTable() {
//    try {
//        showLoadingModal(); // Show the loading modal before the fetch request

//        var startDate = document.getElementById('startDateInput').value;
//        var endDate = document.getElementById('endDateInput').value;
//        var url = '/Home/DisplayElasticData?startDate=' + startDate + '&endDate=' + endDate + '&filterButton=';

//        const response = await fetch(url);

//        if (!response.ok) {
//            throw new Error('Network response was not ok');
//        }

//        const data = await response.json();
//        console.log(data);
//        // Hide the loading modal after the data is loaded and displayed
//        hideLoadingModal();

//    } catch (error) {
//        console.error('Error:', error.message);
//        console.error('Stack Trace:', error.stack);
//        const responseText = await response.text();
//        console.log('Response Text:', responseText);
//        hideLoadingModal(); // Hide the loading modal in case of an error
//    }
//}

//var dataTable; // Declare the variable to hold the DataTable instance

//$(document).ready(function () {
//    var loadingData = false;
//    try {
//        document.getElementById('searchMailNumberInput').addEventListener('input', function () {
//            // Automatically submit the form when the mail number input changes
//            document.getElementById('mailNumberForm').submit();
//        });

//        document.forms["searchForm"].addEventListener('blur', async function (event) {
//            // Prevent the default form submission behavior
//            event.preventDefault();

//            // Get the current value of the hidden input for searchMailNumber
//            var searchMailNumber = document.forms["searchForm"]["searchMailNumber"].value;

//            // If searchMailNumber is not empty, fetch and display the data
//            if (searchMailNumber.trim() !== '') {
//                await loadDataAndDisplayTable(searchMailNumber);
//                // Load and display DataTable after fetching data
//                initializeDataTable();
//            }
//        });

//        document.getElementById('searchSubjectInput').addEventListener('input', async function () {
//            // Get the current value of the searchSubject input
//            var searchSubject = document.forms["searchForm"]["searchSubject"].value;

//            // If searchSubject is not empty, fetch and display the data
//            if (searchSubject.trim() !== '') {
//                try {
//                    showLoadingModal(); // Show loading modal before starting the fetch

//                    // Construct the URL for fetching data based on the searchSubject
//                    var url = '/Home/DisplayElasticData?searchSubject=' + encodeURIComponent(searchSubject);

//                    const response = await fetch(url);

//                    if (!response.ok) {
//                        throw new Error('Network response was not ok');
//                    }

//                    const data = await response.json();
//                    console.log(data);

//                    // Hide the loading modal after the data is loaded and displayed
//                    hideLoadingModal();
//                } catch (error) {
//                    console.error('Error:', error);
//                    hideLoadingModal(); // Hide the loading modal in case of an error
//                }
//                // Load and display DataTable after fetching data
//                initializeDataTable();
//            }
//        });

//        document.getElementById('searchButton').addEventListener('click', function () {
//            handleButtonClick(loadDataAndDisplayTable_);
//        });

//        document.getElementById('filterButton').addEventListener('click', function () {
//            handleButtonClick(loadDataAndDisplayTable);
//        });

//        document.getElementById('mailNumberForm').addEventListener('blur', async function (event) {
//            // Prevent the default form submission behavior
//            event.preventDefault();

//            // Get the current value of the mail number input
//            var mailNumber = document.getElementById('searchMailNumberInput').value;

//            // If the mail number is not empty, fetch and display the data
//            if (mailNumber.trim() !== '') {
//                await loadDataAndDisplayTable(mailNumber);
//                // Load and display DataTable after fetching data
//                initializeDataTable();
//            }
//        });

//    } catch (error) {
//        console.error("An error occurred:", error);
//    }
//});

//// Function to initialize DataTable
//function initializeDataTable() {
//    if (!dataTable) {
//        try {
//            // Prevent DataTable from being initialized more than once
//            dataTable = $("#datatable-buttons").DataTable({
//                scrollY: '100vh', // Enable vertical scrolling
//                lengthChange: true,
//                pageLength: 100,
//                dom: '<"row"<"col-md-6"B><"col-md-6"f>>rt<"row"<"col-md-6"l><"col-md-6"p>>',
//                buttons: [
//                    {
//                        extend: 'excelHtml5',
//                        text: 'Excel',
//                        className: 'btn btn-success btn-sm waves-effect waves-light',
//                        filename: 'ElasticReport'
//                    },
//                ],
//                language: {
//                    search: "Table Filter:"
//                },
//                columnDefs: [{
//                    targets: 0,
//                    orderable: false
//                }],
//                // Callbacks to show/hide loading modal
//                "preInit": function () {
//                    showLoadingModal();
//                },
//                "drawCallback": function () {
//                    hideLoadingModal();
//                }
//            });

//            // Adjust columns without using setTimeout
//            dataTable.columns.adjust().draw();
//        } catch (error) {
//            console.error('Error initializing DataTable:', error);
//        }
//    }
//}

//// --- MAIL NUMBER ---
//async function loadDataAndDisplayTable_(searchMailNumber) {
//    try {
//        showLoadingModal(); // Show loading modal before starting the fetch

//        // Construct the URL for fetching data based on the mail number
//        var url = `/Home/DisplayElasticData?searchMailNumbers=${searchMailNumber}`;

//        const response = await fetch(url);

//        if (!response.ok) {
//            throw new Error('Network response was not ok');
//        }

//        const data = await response.json();
//        console.log(data);

//        // Hide the loading modal after the data is loaded and displayed
//        hideLoadingModal();

//    } catch (error) {
//        console.error('Error:', error.message);
//        console.error('Stack Trace:', error.stack);
//        const responseText = await response.text();
//        console.log('Response Text:', responseText);
//        hideLoadingModal(); // Hide the loading modal in case of an error
//    }
//}

//// --- DATE FILTER ---
//async function loadDataAndDisplayTable() {
//    try {
//        showLoadingModal(); // Show the loading modal before the fetch request

//        var startDate = document.getElementById('startDateInput').value;
//        var endDate = document.getElementById('endDateInput').value;
//        var url = '/Home/DisplayElasticData?startDate=' + startDate + '&endDate=' + endDate + '&filterButton=';

//        const response = await fetch(url);

//        if (!response.ok) {
//            throw new Error('Network response was not ok');
//        }

//        const data = await response.json();
//        console.log(data);

//        // Hide the loading modal after the data is loaded and displayed
//        hideLoadingModal();

//    } catch (error) {
//        console.error('Error:', error.message);
//        console.error('Stack Trace:', error.stack);
//        const responseText = await response.text();
//        console.log('Response Text:', responseText);
//        hideLoadingModal(); // Hide the loading modal in case of an error
//    }
//}

//// Function to show the loading modal
//function showLoadingModal() {
//    var loadingModal = document.getElementById('ProgressModal');
//    loadingModal.style.display = "block";
//}

//// Function to hide the loading modal
//function hideLoadingModal() {
//    var loadingModal = document.getElementById('ProgressModal');
//    loadingModal.style.display = "none";
//}

//// Function to handle button click and prevent multiple clicks
//function handleButtonClick(callback) {
//    if (!loadingData) {
//        loadingData = true;
//        callback();
//        loadingData = false;
//    }
//}