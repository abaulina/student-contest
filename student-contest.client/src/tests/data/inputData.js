export const invalidSignupEntries = [
	{
		email: 'abc.test.com',
		password: '123',
		firstName: '1.',
		lastName: '1User'
	},
	{
		email: 'example@abc',
		password: 'pass12D',
		firstName: 'Test1',
		lastName: 'User'
	},
	{
		email: '.@yahoo.com',
		password: 'pasW34E',
		firstName: 'Test',
		lastName: 'Use1r'
	},
	{
		email: 'us(E)r@testing.com',
		password: 'Pa$$w0r',
		firstName: 'Test',
		lastName: 'User'
	},
	{
		email: 'u$er@testing.com',
		password: '12345678',
		firstName: 'T_est',
		lastName: 'Use_r'
	},
	{
		email: 'u$er@testing.com',
		password: '12345678',
		firstName: 'Test)',
		lastName: 'User'
	},
	{
		email: 'user.@testing.com',
		password: '12345678',
		firstName: 'Test',
		lastName: 'User?'
	},
	{
		email: 'user@.com',
		password: '12345678',
		firstName: 'Test',
		lastName: 'Use-r'
	},
	{
		email: 'user@example.com',
		password: '123457*',
		firstName: 'Test%',
		lastName: 'User@'
	},
	{
		email: 'user@example.com',
		password: '!onetwo~',
		firstName: '_test',
		lastName: 'Use-r'
	},
	{
		email: 'user@example.com',
		password: 'onetwo',
		firstName: 'Test',
		lastName: 'U$er'
	},
	{
		email: 'user@example.com',
		password: 'onetwothree',
		firstName: 'Test%',
		lastName: 'User.'
	},
	{
		email: 'user@.com',
		password: '12345678',
		firstName: 'Test',
		lastName: 'Use-r'
	},
	{
		email: 'user@example.com',
		password: '123678',
		firstName: 'Test',
		lastName: 'User'
	},
	{
		email: 'user@example.com',
		password: '12345678',
		firstName: '.Test',
		lastName: 'Use-r'
	},
	{
		email: 'user@example.com',
		password: '12345678',
		firstName: 'Test',
		lastName: 'User!'
	}
];

export const validUser = {
	email: 'user@example.com',
	password: '12345678',
	firstName: 'Test',
	lastName: 'User'
};

export const invalidLoginEntry = { email: 'example@abc', password: 'pass$12D' };

export const validLoginEntry = {
	email: 'test@example.com',
	password: '12345678'
};

export const notUsedUser = {
	email: 'notUsedAtAll@example.com',
	password: '12345678'
};
