import { login, generateEmail } from './auth';
import { validUser, notUsedUser } from '../data/inputData';
import {
	UserAccountPage,
	MainPage,
	LoginPage,
	SignupPage
} from './pages/index';

fixture`Main page`.page`http://localhost:3000`;

test('navigate to private route requires login success', async (t) => {
	await MainPage.navigateToUrl('../user');

	await LoginPage.assertWelcomeMsgText();
	await login(t);

	await UserAccountPage.assertGreetingMsgText(validUser);
});

test('login page is displaying after logging out', async (t) => {
	await MainPage.navigateToUrl('../user');

	await LoginPage.assertWelcomeMsgText();
	await login(t);

	await UserAccountPage.assertGreetingMsgText(validUser);
	await UserAccountPage.logOut();

	await LoginPage.assertWelcomeMsgText();
});

test('sign up success then login success', async () => {
	const newEmail = generateEmail();

	await LoginPage.clickNoAccountLink();

	await SignupPage.assertCreateAccountMsgText();

	await SignupPage.assertFirstNameInputExists();
	await SignupPage.typeFirstName(validUser.firstName);
	await SignupPage.assertFirstNameValue(validUser.firstName);

	await SignupPage.assertLastNameInputExists();
	await SignupPage.typeLastName(validUser.lastName);
	await SignupPage.assertLastNameValue(validUser.lastName);

	await SignupPage.assertEmailInputExists();
	await SignupPage.typeEmail(newEmail);
	await SignupPage.assertEmailValue(newEmail);

	await SignupPage.assertPasswordInputExists();
	await SignupPage.typePassword(validUser.password);
	await SignupPage.assertPasswordValue(validUser.password);

	await SignupPage.clickSignupButton();
	await SignupPage.assertSignupSuccessMsgText();

	await SignupPage.clickGoToLoginButton();

	await LoginPage.typeEmail(newEmail);
	await LoginPage.typePassword(validUser.password);
	await LoginPage.clickSubmitButton();

	await UserAccountPage.assertGreetingMsgExists();
});

test('impossible to register with duplicate email', async () => {
	await MainPage.clickSignupButton();

	await SignupPage.assertCreateAccountMsgText();

	await SignupPage.assertFirstNameInputExists();
	await SignupPage.typeFirstName(validUser.firstName);
	await SignupPage.assertFirstNameValue(validUser.firstName);

	await SignupPage.assertLastNameInputExists();
	await SignupPage.typeLastName(validUser.lastName);
	await SignupPage.assertLastNameValue(validUser.lastName);

	await SignupPage.assertEmailInputExists();
	await SignupPage.typeEmail(validUser.email);
	await SignupPage.assertEmailValue(validUser.email);

	await SignupPage.assertPasswordInputExists();
	await SignupPage.typePassword(validUser.password);
	await SignupPage.assertPasswordValue(validUser.password);

	await SignupPage.clickSignupButton();
	await SignupPage.assertInvalidFeedback();
});

test('impossible to login with not used email', async () => {
	await LoginPage.typeEmail(notUsedUser.email);
	await LoginPage.typePassword(notUsedUser.password);

	await LoginPage.clickSubmitButton();
	await LoginPage.assertInvalidFeedback();
});
