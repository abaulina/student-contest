import { Selector, t } from 'testcafe';

const UserAccountPage = () => {
	const greetingMsg = Selector('p');
	const logOutButton = Selector('button').withText('Log out');

	return {
		async assertGreetingMsgText(validUser) {
			await t
				.expect(greetingMsg.innerText)
				.eql(
					'Nice to see you again, ' +
						validUser.lastName +
						' ' +
						validUser.firstName
				);
		},

		async assertGreetingMsgExists() {
			await t.expect(greetingMsg.innerText).contains('Nice to see you again, ');
		},

		async logOut() {
			await t.click(logOutButton);
		}
	};
};

export default UserAccountPage();
