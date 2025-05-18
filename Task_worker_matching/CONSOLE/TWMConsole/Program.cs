using Task_worker_matching.ServiceLayer;

AccountService AS = new();

LoginMenu L = new(AS);

SignUpMenu s = new(AS);

s.signup();
L.login();