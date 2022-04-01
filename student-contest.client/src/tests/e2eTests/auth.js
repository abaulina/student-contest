import { Selector } from 'testcafe';
import { validUser } from '../data/inputData';

export const login = async (t) => {
	await t
		.typeText('#floatingEmail', validUser.email)
		.typeText('#floatingPassword', validUser.password)
		.click(Selector('button').withText('Submit'));
};
