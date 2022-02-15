import { Selector } from 'testcafe';
import { login, loginAfterSignup } from './auth';
import { validUser } from '../data/inputData';

fixture`Main page`.page`http://localhost:3000`;

test('navigate to private route requires login success', async (t) => {
	await t
		.navigateTo('../user')
		.expect(Selector('h3').innerText)
		.eql('Welcome Back');
	await login(t);
	await t
		.expect(Selector('p').innerText)
		.eql(
			'Nice to see you again, ' + validUser.firstName + ' ' + validUser.lastName
		);
});

test('login page is displaying after logging out', async (t) => {
	await t
		.navigateTo('../user')
		.expect(Selector('h3').innerText)
		.eql('Welcome Back');
	await login(t);
	await t
		.expect(Selector('p').innerText)
		.eql(
			'Nice to see you again, ' + validUser.firstName + ' ' + validUser.lastName
		)

		.click(Selector('button').withText('Log out'))
		.expect(Selector('h3').innerText)
		.eql('Welcome Back');
});

test('sign up success then login success', async (t) => {
	const firstNameInput = Selector('#floatingFirstName');
	const lastNameInput = Selector('#floatingLastName');
	const emailInput = Selector('#floatingEmail');
	const passwordInput = Selector('#floatingPassword');

	await t
		.click(Selector('a').withText('No account'))
		.expect(Selector('h3').innerText)
		.eql('Create account')

		.expect(firstNameInput.exists)
		.ok()
		.typeText(firstNameInput, validUser.firstName)
		.expect(firstNameInput.value)
		.eql(validUser.firstName)

		.expect(lastNameInput.exists)
		.ok()
		.typeText(lastNameInput, validUser.lastName)
		.expect(lastNameInput.value)
		.eql(validUser.lastName)

		.expect(emailInput.exists)
		.ok()
		.typeText(emailInput, validUser.email)
		.expect(emailInput.value)
		.eql(validUser.email)

		.expect(passwordInput.exists)
		.ok()
		.typeText(passwordInput, validUser.password)
		.expect(passwordInput.value)
		.eql(validUser.password)

		.click('button.default')
		.expect(Selector('h3').innerText)
		.eql('Thanks for signing up')

		.click(Selector('a').withText('Log in'));
	await loginAfterSignup(t);
	await t.expect(Selector('p').innerText).contains('Nice to see you again,');
});