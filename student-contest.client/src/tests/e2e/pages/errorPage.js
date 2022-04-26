import { Selector, t } from 'testcafe';

const ErrorPage = () => {
	const errorMsg = Selector('p');
	const goToMainPageButton = Selector('a.not-found');

	return {
		async assertError404MsgText() {
			await t.expect(errorMsg.innerText).contains('removed');
		},

		async assertErrorDefaultMsgText() {
			await t.expect(errorMsg.innerText).contains('try again');
		},

		async assertGoToMainPageButtonText() {
			await t
				.expect(goToMainPageButton.innerText)
				.eql('Take me back to the homepage');
		}
	};
};

export default ErrorPage();
