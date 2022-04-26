import { Selector, t } from 'testcafe';

const MainPage = () => {
	const signupButton = Selector('button').withText('Sign up');

	return {
		async navigateToUrl(pageUrl) {
			await t.navigateTo(pageUrl);
		},

		async clickSignupButton() {
			await t.click(signupButton);
		}
	};
};

export default MainPage();
