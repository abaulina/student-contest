import { Selector, t } from 'testcafe';

const SignupPage = () => {
	const firstNameInput = Selector('#floatingFirstName');
	const lastNameInput = Selector('#floatingLastName');
	const emailInput = Selector('#floatingEmail');
	const passwordInput = Selector('#floatingPassword');

	const signupButton = Selector('button').withText('Sign Up');
	const invalidFeedback = Selector('.invalid-feedback');
	const createAccountMsg = Selector('h3');
	const signupSuccessMsg = Selector('h3');

	const goToLoginButton = Selector('a').withText('Log in');

	return {
		async typeEmail(email) {
			await t.typeText(emailInput, email);
		},

		async typeFirstName(firstName) {
			await t.typeText(firstNameInput, firstName);
		},

		async typeLastName(lastName) {
			await t.typeText(lastNameInput, lastName);
		},

		async typePassword(password) {
			await t.typeText(passwordInput, password);
		},

		async assertFirstNameInputExists() {
			await t.expect(firstNameInput.exists).ok();
		},

		async assertLastNameInputExists() {
			await t.expect(lastNameInput.exists).ok();
		},

		async assertEmailInputExists() {
			await t.expect(emailInput.exists).ok();
		},

		async assertPasswordInputExists() {
			await t.expect(passwordInput.exists).ok();
		},

		async assertFirstNameValue(firstName) {
			await t.expect(firstNameInput.value).eql(firstName);
		},

		async assertLastNameValue(lastName) {
			await t.expect(lastNameInput.value).eql(lastName);
		},

		async assertEmailValue(email) {
			await t.expect(emailInput.value).eql(email);
		},

		async assertPasswordValue(password) {
			await t.expect(passwordInput.value).eql(password);
		},

		async clickSignupButton() {
			await t.click(signupButton);
		},

		async clickGoToLoginButton() {
			await t.click(goToLoginButton);
		},

		async assertInvalidFeedback() {
			await t.expect(invalidFeedback.innerText).eql('Email is invalid');
		},

		async assertCreateAccountMsgText() {
			await t.expect(createAccountMsg.innerText).eql('Create account');
		},

		async assertSignupSuccessMsgText() {
			await t.expect(signupSuccessMsg.innerText).eql('Thanks for signing up');
		}
	};
};

export default SignupPage();
