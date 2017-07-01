/** 
 * Bundle rules
 */
OpenIZBre.AddBusinessRule("Bundle", "BeforeInsert", /** @param {OpenIZModel.Bundle} bundle */ function (bundle) {
    if (bundle.$type != 'Bundle') return bundle;
    return OpenIZBre.ExecuteBundleRules("BeforeInsert", bundle);
});
/** 
 * Bundle rules
 */
OpenIZBre.AddBusinessRule("Bundle", "AfterInsert", /** @param {OpenIZModel.Bundle} bundle */ function (bundle) {
    if (bundle.$type != 'Bundle') return bundle;

    return OpenIZBre.ExecuteBundleRules("AfterInsert", bundle);
});

