import { Selector, t } from 'testcafe';

const LoginPage = () => {
	const emailInput = Selector('#floatingEmail');
	const passwordInput = Selector('#floatingPassword');

	const invalidFeedback = Selector('.invalid-feedback');
	const submitButton = Selector('button').withText('Submit');
	const welcomeMsg = Selector('h3');
	const noAccountLink = Selector('a').withText('No account');

	return {
		async typeEmail(email) {
			await t.typeText(emailInput, email);
		},

		async typePassword(password) {
			await t.typeText(passwordInput, password);
		},

		async clickSubmitButton() {
			await t.click(submitButton);
		},

		async assertInvalidFeedback() {
			await t
				.expect(invalidFeedback.innerText)
				.eql('Invalid email or password');
		},

		async assertWelcomeMsgText() {
			await t.expect(welcomeMsg.innerText).eql('Welcome Back');
		},

		async clickNoAccountLink() {
			await t.click(noAccountLink);
		}
	};
};

export default LoginPage();
