// API Base URL
const API_URL = '/api';

// State
let token = localStorage.getItem('token');
let currentUser = JSON.parse(localStorage.getItem('user') || 'null');

// Initialize
document.addEventListener('DOMContentLoaded', () => {
    if (token && currentUser) {
        showMainSection();
    } else {
        showAuthSection();
    }
});

// Auth Functions
function showLogin() {
    document.getElementById('loginForm').style.display = 'flex';
    document.getElementById('registerForm').style.display = 'none';
    document.querySelectorAll('.auth-tabs .tab-btn').forEach((btn, i) => {
        btn.classList.toggle('active', i === 0);
    });
}

function showRegister() {
    document.getElementById('loginForm').style.display = 'none';
    document.getElementById('registerForm').style.display = 'flex';
    document.querySelectorAll('.auth-tabs .tab-btn').forEach((btn, i) => {
        btn.classList.toggle('active', i === 1);
    });
}

async function login() {
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;
    const errorDiv = document.getElementById('loginError');

    try {
        const response = await fetch(`${API_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        const data = await response.json();

        if (response.ok) {
            token = data.token;
            currentUser = data;
            localStorage.setItem('token', token);
            localStorage.setItem('user', JSON.stringify(currentUser));
            showMainSection();
        } else {
            showError(errorDiv, data);
        }
    } catch (error) {
        showError(errorDiv, 'Failed to connect to server');
    }
}

async function register() {
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;
    const firstName = document.getElementById('registerFirstName').value;
    const lastName = document.getElementById('registerLastName').value;
    const errorDiv = document.getElementById('registerError');

    try {
        const response = await fetch(`${API_URL}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password, firstName, lastName })
        });

        const data = await response.json();

        if (response.ok) {
            token = data.token;
            currentUser = data;
            localStorage.setItem('token', token);
            localStorage.setItem('user', JSON.stringify(currentUser));
            showMainSection();
        } else {
            showError(errorDiv, data);
        }
    } catch (error) {
        showError(errorDiv, 'Failed to connect to server');
    }
}

function logout() {
    token = null;
    currentUser = null;
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    showAuthSection();
}

function showAuthSection() {
    document.getElementById('authSection').style.display = 'block';
    document.getElementById('mainSection').style.display = 'none';
}

function showMainSection() {
    document.getElementById('authSection').style.display = 'none';
    document.getElementById('mainSection').style.display = 'block';
    document.getElementById('userInfo').textContent = `${currentUser.firstName} ${currentUser.lastName} (${currentUser.email})`;
}

// Tab Navigation
function showTab(tabName) {
    document.querySelectorAll('.tab-content').forEach(tab => tab.style.display = 'none');
    document.querySelectorAll('.main-tabs .tab-btn').forEach(btn => btn.classList.remove('active'));

    document.getElementById(tabName + 'Tab').style.display = 'block';
    event.target.classList.add('active');
}

// Customer Functions
function showCreateCustomerForm() {
    document.getElementById('createCustomerForm').style.display = 'grid';
}

function cancelCreateCustomer() {
    document.getElementById('createCustomerForm').style.display = 'none';
    clearCustomerForm();
}

function clearCustomerForm() {
    document.getElementById('customerUsername').value = '';
    document.getElementById('customerFirstName').value = '';
    document.getElementById('customerLastName').value = '';
    document.getElementById('customerEgn').value = '';
    document.getElementById('customerPhone').value = '';
    document.getElementById('customerEmail').value = '';
    document.getElementById('customerStreet').value = '';
    document.getElementById('customerCity').value = '';
    document.getElementById('customerZipCode').value = '';
    document.getElementById('customerCountry').value = '';
}

async function createCustomer() {
    const data = {
        username: document.getElementById('customerUsername').value,
        firstName: document.getElementById('customerFirstName').value,
        lastName: document.getElementById('customerLastName').value,
        egn: document.getElementById('customerEgn').value,
        phoneNumber: document.getElementById('customerPhone').value,
        email: document.getElementById('customerEmail').value,
        address: {
            street: document.getElementById('customerStreet').value,
            city: document.getElementById('customerCity').value,
            zipCode: document.getElementById('customerZipCode').value,
            country: document.getElementById('customerCountry').value
        }
    };

    const errorDiv = document.getElementById('createCustomerError');

    try {
        const response = await apiCall('/customers/create', 'POST', data);

        if (response.ok) {
            const result = await response.json();
            cancelCreateCustomer();
            showSuccess('Customer created successfully!');
            displayCustomer(result);
        } else {
            const error = await response.json();
            showError(errorDiv, error);
        }
    } catch (error) {
        showError(errorDiv, 'Failed to create customer');
    }
}

async function searchCustomerByEgn() {
    const egn = document.getElementById('searchEgn').value;

    try {
        const response = await apiCall(`/customers/egn/${egn}`, 'GET');

        if (response.ok) {
            const result = await response.json();
            displayCustomer(result);
        } else {
            document.getElementById('customersResult').innerHTML = '<p class="error show">Customer not found</p>';
        }
    } catch (error) {
        document.getElementById('customersResult').innerHTML = '<p class="error show">Failed to search customer</p>';
    }
}

function displayCustomer(customer) {
    const html = `
        <div class="card">
            <h3>Customer: ${customer.firstName} ${customer.lastName}</h3>
            <div class="info-row">
                <span class="label">ID:</span>
                <span class="value">${customer.id}</span>
            </div>
            <div class="info-row">
                <span class="label">Username:</span>
                <span class="value">${customer.username}</span>
            </div>
            <div class="info-row">
                <span class="label">EGN:</span>
                <span class="value">${customer.egn}</span>
            </div>
            <div class="info-row">
                <span class="label">Phone:</span>
                <span class="value">${customer.phoneNumber}</span>
            </div>
            <div class="info-row">
                <span class="label">Email:</span>
                <span class="value">${customer.email}</span>
            </div>
            <div class="info-row">
                <span class="label">Address:</span>
                <span class="value">${customer.address.street}, ${customer.address.city}, ${customer.address.zipCode}, ${customer.address.country}</span>
            </div>
            <div class="info-row">
                <span class="label">Status:</span>
                <span class="value"><span class="status ${customer.isActive ? 'active' : 'closed'}">${customer.isActive ? 'Active' : 'Inactive'}</span></span>
            </div>
        </div>
    `;
    document.getElementById('customersResult').innerHTML = html;
}

// Account Functions
function showOpenAccountForm() {
    document.getElementById('openAccountForm').style.display = 'grid';
}

function cancelOpenAccount() {
    document.getElementById('openAccountForm').style.display = 'none';
}

async function openAccount() {
    const data = {
        customerId: document.getElementById('accountCustomerId').value,
        accountType: parseInt(document.getElementById('accountType').value),
        initialBalance: parseFloat(document.getElementById('initialBalance').value),
        withdrawLimit: parseFloat(document.getElementById('withdrawLimit').value) || null,
        depositTerm: parseInt(document.getElementById('depositTerm').value) || null
    };

    const errorDiv = document.getElementById('openAccountError');

    try {
        const response = await apiCall('/accounts/open', 'POST', data);

        if (response.ok) {
            const result = await response.json();
            cancelOpenAccount();
            showSuccess('Account opened successfully!');
            displayAccount(result);
        } else {
            const error = await response.json();
            showError(errorDiv, error);
        }
    } catch (error) {
        showError(errorDiv, 'Failed to open account');
    }
}

async function getCustomerAccounts() {
    const customerId = document.getElementById('searchCustomerId').value;

    try {
        const response = await apiCall(`/accounts/customer/${customerId}`, 'GET');

        if (response.ok) {
            const accounts = await response.json();
            displayAccounts(accounts);
        } else {
            document.getElementById('accountsResult').innerHTML = '<p class="error show">No accounts found</p>';
        }
    } catch (error) {
        document.getElementById('accountsResult').innerHTML = '<p class="error show">Failed to get accounts</p>';
    }
}

function displayAccount(account) {
    const html = `
        <div class="card">
            <h3>${getAccountTypeName(account.accountType)} Account</h3>
            <div class="info-row">
                <span class="label">Account ID:</span>
                <span class="value">${account.id}</span>
            </div>
            <div class="info-row">
                <span class="label">IBAN:</span>
                <span class="value">${account.iban}</span>
            </div>
            <div class="info-row">
                <span class="label">Balance:</span>
                <span class="value">$${account.balance.toFixed(2)}</span>
            </div>
            <div class="info-row">
                <span class="label">Status:</span>
                <span class="value"><span class="status ${account.status.toLowerCase()}">${account.status}</span></span>
            </div>
        </div>
    `;
    document.getElementById('accountsResult').innerHTML = html;
}

function displayAccounts(accounts) {
    if (!accounts || accounts.length === 0) {
        document.getElementById('accountsResult').innerHTML = '<p>No accounts found</p>';
        return;
    }

    const html = accounts.map(account => `
        <div class="card">
            <h3>${getAccountTypeName(account.accountType)} Account</h3>
            <div class="info-row">
                <span class="label">Account ID:</span>
                <span class="value">${account.id}</span>
            </div>
            <div class="info-row">
                <span class="label">IBAN:</span>
                <span class="value">${account.iban}</span>
            </div>
            <div class="info-row">
                <span class="label">Balance:</span>
                <span class="value">$${account.balance.toFixed(2)}</span>
            </div>
            <div class="info-row">
                <span class="label">Status:</span>
                <span class="value"><span class="status ${account.status.toLowerCase()}">${account.status}</span></span>
            </div>
        </div>
    `).join('');

    document.getElementById('accountsResult').innerHTML = html;
}

function getAccountTypeName(type) {
    const types = { 0: 'Checking', 1: 'Saving', 2: 'Deposit' };
    return types[type] || 'Unknown';
}

// Transaction Functions
function showDepositForm() {
    hideAllTransactionForms();
    document.getElementById('depositForm').style.display = 'grid';
}

function showWithdrawForm() {
    hideAllTransactionForms();
    document.getElementById('withdrawForm').style.display = 'grid';
}

function showTransferForm() {
    hideAllTransactionForms();
    document.getElementById('transferForm').style.display = 'grid';
}

function hideAllTransactionForms() {
    document.getElementById('depositForm').style.display = 'none';
    document.getElementById('withdrawForm').style.display = 'none';
    document.getElementById('transferForm').style.display = 'none';
}

function cancelDeposit() {
    document.getElementById('depositForm').style.display = 'none';
}

function cancelWithdraw() {
    document.getElementById('withdrawForm').style.display = 'none';
}

function cancelTransfer() {
    document.getElementById('transferForm').style.display = 'none';
}

async function deposit() {
    const customerId = document.getElementById('depositCustomerId').value;
    const accountId = document.getElementById('depositAccountId').value;
    const amount = parseFloat(document.getElementById('depositAmount').value);

    const errorDiv = document.getElementById('depositError');

    try {
        const formData = new FormData();
        formData.append('customerId', customerId);
        formData.append('accountId', accountId);
        formData.append('amount', amount);

        const response = await fetch(`${API_URL}/customers/deposit`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
            },
            body: formData
        });

        if (response.ok) {
            cancelDeposit();
            showSuccess('Deposit successful!');
        } else {
            const error = await response.json();
            showError(errorDiv, error);
        }
    } catch (error) {
        showError(errorDiv, 'Failed to deposit');
    }
}

async function withdraw() {
    const data = {
        customerId: document.getElementById('withdrawCustomerId').value,
        accountId: document.getElementById('withdrawAccountId').value,
        amount: parseFloat(document.getElementById('withdrawAmount').value)
    };

    const errorDiv = document.getElementById('withdrawError');

    try {
        const response = await apiCall('/customers/withdraw', 'POST', data);

        if (response.ok) {
            cancelWithdraw();
            showSuccess('Withdrawal successful!');
        } else {
            const error = await response.json();
            showError(errorDiv, error);
        }
    } catch (error) {
        showError(errorDiv, 'Failed to withdraw');
    }
}

async function transfer() {
    const data = {
        senderCustomerId: document.getElementById('transferSenderCustomerId').value,
        fromAccountId: document.getElementById('transferFromAccountId').value,
        receiverCustomerId: document.getElementById('transferReceiverCustomerId').value,
        toAccountId: document.getElementById('transferToAccountId').value,
        amount: parseFloat(document.getElementById('transferAmount').value)
    };

    const errorDiv = document.getElementById('transferError');

    try {
        const response = await apiCall('/customers/transfer', 'POST', data);

        if (response.ok) {
            cancelTransfer();
            showSuccess('Transfer successful!');
        } else {
            const error = await response.json();
            showError(errorDiv, error);
        }
    } catch (error) {
        showError(errorDiv, 'Failed to transfer');
    }
}

async function getAccountTransactions() {
    const accountId = document.getElementById('transactionAccountId').value;

    try {
        const response = await apiCall(`/transactions/account/${accountId}`, 'GET');

        if (response.ok) {
            const transactions = await response.json();
            displayTransactions(transactions);
        } else {
            document.getElementById('transactionsResult').innerHTML = '<p class="error show">No transactions found</p>';
        }
    } catch (error) {
        document.getElementById('transactionsResult').innerHTML = '<p class="error show">Failed to get transactions</p>';
    }
}

function displayTransactions(transactions) {
    if (!transactions || transactions.length === 0) {
        document.getElementById('transactionsResult').innerHTML = '<p>No transactions found</p>';
        return;
    }

    const html = `
        <table>
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Type</th>
                    <th>Description</th>
                    <th>Amount</th>
                </tr>
            </thead>
            <tbody>
                ${transactions.map(tx => `
                    <tr>
                        <td>${new Date(tx.timestamp).toLocaleString()}</td>
                        <td>${tx.transactionType}</td>
                        <td>${tx.description || '-'}</td>
                        <td>$${tx.amount ? tx.amount.toFixed(2) : '0.00'}</td>
                    </tr>
                `).join('')}
            </tbody>
        </table>
    `;

    document.getElementById('transactionsResult').innerHTML = html;
}

// Utility Functions
async function apiCall(endpoint, method = 'GET', body = null) {
    const options = {
        method,
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    };

    if (body && method !== 'GET') {
        options.body = JSON.stringify(body);
    }

    const response = await fetch(API_URL + endpoint, options);

    if (response.status === 401) {
        logout();
        throw new Error('Unauthorized');
    }

    return response;
}

function showError(element, message) {
    element.textContent = typeof message === 'string' ? message : JSON.stringify(message);
    element.classList.add('show');
    setTimeout(() => element.classList.remove('show'), 5000);
}

function showSuccess(message) {
    const successDiv = document.createElement('div');
    successDiv.className = 'success';
    successDiv.textContent = message;
    document.body.appendChild(successDiv);
    setTimeout(() => successDiv.remove(), 3000);
}
